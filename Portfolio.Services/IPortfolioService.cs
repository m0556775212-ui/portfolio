using System.Collections.Generic;
using System.Threading.Tasks;

namespace Portfolio.Services
{
    public interface IPortfolioService
    {
        Task<List<RepositoryDto>> GetPortfolio();
        Task<List<RepositoryDto>> SearchRepositories(string? name = null, string? language = null, string? user = null);
    }
}
