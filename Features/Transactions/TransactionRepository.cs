using MongoDB.Driver;
using portfolio_service.Models;
using portfolio_service.Services;

namespace portfolio_service.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(string id);
        Task<IEnumerable<Transaction>> GetByPortfolioIdAsync(string portfolioId);
        Task<IEnumerable<Transaction>> GetByUserIdAsync(string userId);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(string id, UpdateTransactionRequest request);
        Task<bool> DeleteAsync(string id);
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMongoCollection<Transaction> _transactions;

        public TransactionRepository(IMongoService mongoService)
        {
            var database = mongoService.GetDatabase();
            _transactions = database.GetCollection<Transaction>("transactions");
        }

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            var filter = Builders<Transaction>.Filter.Eq(t => t.Id, id);
            return await _transactions.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByPortfolioIdAsync(string portfolioId)
        {
            var filter = Builders<Transaction>.Filter.Eq(t => t.PortfolioId, portfolioId);
            var sort = Builders<Transaction>.Sort.Descending(t => t.Date);
            
            return await _transactions.Find(filter).Sort(sort).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<Transaction>.Filter.Eq(t => t.UserId, userId);
            var sort = Builders<Transaction>.Sort.Descending(t => t.Date);
            
            return await _transactions.Find(filter).Sort(sort).ToListAsync();
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            await _transactions.InsertOneAsync(transaction);
            return transaction;
        }

        public async Task<Transaction?> UpdateAsync(string id, UpdateTransactionRequest request)
        {
            var filter = Builders<Transaction>.Filter.Eq(t => t.Id, id);

            var update = Builders<Transaction>.Update.Set(t => t.UpdatedAt, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(request.Symbol))
            {
                update = update.Set(t => t.Symbol, request.Symbol);
            }

            if (request.Quantity.HasValue)
            {
                update = update.Set(t => t.Quantity, request.Quantity.Value);
            }

            if (request.PricePaid.HasValue)
            {
                update = update.Set(t => t.PricePaid, request.PricePaid.Value);
            }

            if (request.Date.HasValue)
            {
                update = update.Set(t => t.Date, request.Date.Value);
            }

            if (request.TransactionType.HasValue)
            {
                update = update.Set(t => t.TransactionType, request.TransactionType.Value);
            }

            if (request.Notes != null)
            {
                update = update.Set(t => t.Notes, request.Notes);
            }

            var options = new FindOneAndUpdateOptions<Transaction>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _transactions.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<Transaction>.Filter.Eq(t => t.Id, id);
            var result = await _transactions.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}
