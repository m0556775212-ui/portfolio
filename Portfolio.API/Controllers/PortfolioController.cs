using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Services;

namespace Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RepositoryDto>>> GetPortfolio()
        {
            var portfolio = await _portfolioService.GetPortfolio();
            return Ok(portfolio);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<RepositoryDto>>> Search([FromQuery] string? name, [FromQuery] string? language, [FromQuery] string? user)
        {
            var results = await _portfolioService.SearchRepositories(name, language, user);
            return Ok(results);
        }
    }
}
