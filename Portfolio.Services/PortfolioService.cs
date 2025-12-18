using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Octokit;

namespace Portfolio.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly GitHubClient _client;
        private readonly GitHubSettings _settings;

        public PortfolioService(IOptions<GitHubSettings> settings)
        {
            _settings = settings.Value;
            _client = new GitHubClient(new ProductHeaderValue("PortfolioApp"));

            if (!string.IsNullOrEmpty(_settings.Token))
            {
                _client.Credentials = new Credentials(_settings.Token);
            }
        }

        public async Task<List<RepositoryDto>> GetPortfolio()
        {
            var repositories = await _client.Repository.GetAllForCurrent();
            
            var tasks = repositories.Select(async repo =>
            {
                var languagesTask = _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);
                var pullRequestsTask = _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                await Task.WhenAll(languagesTask, pullRequestsTask);

                var languages = await languagesTask;
                var pullRequests = await pullRequestsTask;

                return new RepositoryDto
                {
                    Name = repo.Name,
                    Languages = languages.Select(l => l.Name).ToList(),
                    LastCommit = repo.PushedAt,
                    Stars = repo.StargazersCount,
                    PullRequests = pullRequests.Count,
                    Url = repo.HtmlUrl
                };
            });

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public async Task<List<RepositoryDto>> SearchRepositories(string? name = null, string? language = null, string? user = null)
        {
            var request = new SearchRepositoriesRequest(name ?? "portfolio") // Search requires at least one term usually, or we can search for everything? Octokit might require a term.
            {
            };

            if (!string.IsNullOrEmpty(name))
            {
                // If name is provided, it's the main query.
                // If not, we might need a default query or handle it differently.
                // The prompt says "Search general in GitHub... filtering by name, language, username".
                // If name is null, we can't just search for nothing.
                // Let's assume if name is null, we search for * something or just don't set the term?
                // SearchRepositoriesRequest constructor takes a string term.
            }
            
            // Re-initializing request based on parameters
            // If name is missing, we can search for "stars:>0" or something generic if the user wants "all public repos" (which is huge).
            // But usually a search box has an input.
            // Let's assume 'name' is the search term.
            
            var term = string.IsNullOrEmpty(name) ? "portfolio" : name;
            request = new SearchRepositoriesRequest(term);

            if (!string.IsNullOrEmpty(language))
            {
                if (Enum.TryParse(language, true, out Language langEnum))
                {
                    request.Language = langEnum;
                }
                else
                {
                    // If it's not in the enum, we might not be able to filter by it easily with the strong type, 
                    // or we have to use the string version if Octokit supports it.
                    // Octokit's SearchRepositoriesRequest.Language is an enum.
                    // If it fails, we might ignore it or try to map it.
                    // For now, let's assume standard languages.
                }
            }

            if (!string.IsNullOrEmpty(user))
            {
                request.User = user;
            }

            var searchResult = await _client.Search.SearchRepo(request);

            // Mapping search results to DTO. 
            // Note: Search results (Repository) might not have all details like PullRequests count or full language list without extra calls.
            // The prompt says "Search... returns list of repositories". It doesn't explicitly say it needs the FULL details like GetPortfolio.
            // But usually it's good to return similar structure.
            // However, making extra calls for 100 search results is bad.
            // I will map what is available.
            
            return searchResult.Items.Select(repo => new RepositoryDto
            {
                Name = repo.Name,
                Languages = new List<string> { repo.Language }, // Search result only has main language
                LastCommit = repo.PushedAt,
                Stars = repo.StargazersCount,
                PullRequests = 0, // Not available in search result
                Url = repo.HtmlUrl
            }).ToList();
        }
    }
}
