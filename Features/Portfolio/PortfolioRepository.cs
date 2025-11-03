using MongoDB.Driver;
using portfolio_service.Models;
using portfolio_service.Services;

namespace portfolio_service.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetByIdAsync(string id);
        Task<IEnumerable<Portfolio>> GetByUserIdAsync(string userId);
        Task<Portfolio> CreateAsync(Portfolio portfolio);
        Task<Portfolio?> UpdateAsync(string id, UpdatePortfolioRequest request);
        Task<bool> DeleteAsync(string id);
    }

    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IMongoCollection<Portfolio> _portfolios;

        public PortfolioRepository(IMongoService mongoService)
        {
            var database = mongoService.GetDatabase();
            _portfolios = database.GetCollection<Portfolio>("portfolios");
        }

        public async Task<Portfolio?> GetByIdAsync(string id)
        {
            var filter = Builders<Portfolio>.Filter.And(
                Builders<Portfolio>.Filter.Eq(p => p.Id, id),
                Builders<Portfolio>.Filter.Eq(p => p.IsActive, true)
            );
            
            return await _portfolios.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Portfolio>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<Portfolio>.Filter.And(
                Builders<Portfolio>.Filter.Eq(p => p.UserId, userId),
                Builders<Portfolio>.Filter.Eq(p => p.IsActive, true)
            );
            
            var sort = Builders<Portfolio>.Sort.Descending(p => p.CreatedAt);
            
            return await _portfolios.Find(filter).Sort(sort).ToListAsync();
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _portfolios.InsertOneAsync(portfolio);
            return portfolio;
        }

        public async Task<Portfolio?> UpdateAsync(string id, UpdatePortfolioRequest request)
        {
            var filter = Builders<Portfolio>.Filter.And(
                Builders<Portfolio>.Filter.Eq(p => p.Id, id),
                Builders<Portfolio>.Filter.Eq(p => p.IsActive, true)
            );

            var update = Builders<Portfolio>.Update.Set(p => p.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(request.Name))
            {
                update = update.Set(p => p.Name, request.Name);
            }

            if (request.Description != null)
            {
                update = update.Set(p => p.Description, request.Description);
            }

            var options = new FindOneAndUpdateOptions<Portfolio>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _portfolios.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<Portfolio>.Filter.Eq(p => p.Id, id);
            var update = Builders<Portfolio>.Update
                .Set(p => p.IsActive, false)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _portfolios.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
