using portfolio_service.Models;
using portfolio_service.Repositories;

namespace portfolio_service.Services
{
    public interface IPortfolioService
    {
        Task<PortfolioDto?> GetByIdAsync(string id);
        Task<IEnumerable<PortfolioDto>> GetByUserIdAsync(string userId);
        Task<PortfolioDto> CreateAsync(CreatePortfolioRequest request);
        Task<PortfolioDto?> UpdateAsync(string id, UpdatePortfolioRequest request);
        Task<bool> DeleteAsync(string id);
    }

    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioService(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task<PortfolioDto?> GetByIdAsync(string id)
        {
            var portfolio = await _portfolioRepository.GetByIdAsync(id);
            return portfolio != null ? MapToDto(portfolio) : null;
        }

        public async Task<IEnumerable<PortfolioDto>> GetByUserIdAsync(string userId)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(userId);
            return portfolios.Select(MapToDto);
        }

        public async Task<PortfolioDto> CreateAsync(CreatePortfolioRequest request)
        {
            var portfolio = new Portfolio
            {
                Name = request.Name,
                UserId = request.UserId,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdPortfolio = await _portfolioRepository.CreateAsync(portfolio);
            return MapToDto(createdPortfolio);
        }

        public async Task<PortfolioDto?> UpdateAsync(string id, UpdatePortfolioRequest request)
        {
            var portfolio = await _portfolioRepository.UpdateAsync(id, request);
            return portfolio != null ? MapToDto(portfolio) : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _portfolioRepository.DeleteAsync(id);
        }

        private static PortfolioDto MapToDto(Portfolio portfolio)
        {
            return new PortfolioDto
            {
                Id = portfolio.Id,
                Name = portfolio.Name,
                UserId = portfolio.UserId,
                Description = portfolio.Description,
                IsActive = portfolio.IsActive,
                CreatedAt = portfolio.CreatedAt,
                UpdatedAt = portfolio.UpdatedAt
            };
        }
    }
}
