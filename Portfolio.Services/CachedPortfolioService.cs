using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Portfolio.Services
{
    public class CachedPortfolioService : IPortfolioService
    {
        private readonly IPortfolioService _innerService;
        private readonly IMemoryCache _cache;
        private const string PortfolioCacheKey = "PortfolioData";

        public CachedPortfolioService(IPortfolioService innerService, IMemoryCache cache)
        {
            _innerService = innerService;
            _cache = cache;
        }

        public async Task<List<RepositoryDto>> GetPortfolio()
        {
            if (!_cache.TryGetValue(PortfolioCacheKey, out List<RepositoryDto>? portfolio))
            {
                portfolio = await _innerService.GetPortfolio();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Cache for 5 minutes

                _cache.Set(PortfolioCacheKey, portfolio, cacheEntryOptions);
            }

            return portfolio ?? new List<RepositoryDto>();
        }

        public Task<List<RepositoryDto>> SearchRepositories(string? name = null, string? language = null, string? user = null)
        {
            return _innerService.SearchRepositories(name, language, user);
        }
    }
}
