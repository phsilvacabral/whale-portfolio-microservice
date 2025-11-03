using portfolio_service.Models;
using portfolio_service.Repositories;

namespace portfolio_service.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto?> GetByIdAsync(string id);
        Task<IEnumerable<TransactionDto>> GetByPortfolioIdAsync(string portfolioId);
        Task<IEnumerable<TransactionDto>> GetByUserIdAsync(string userId);
        Task<TransactionDto> CreateAsync(CreateTransactionRequest request);
        Task<TransactionDto?> UpdateAsync(string id, UpdateTransactionRequest request);
        Task<bool> DeleteAsync(string id);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionDto?> GetByIdAsync(string id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return transaction != null ? MapToDto(transaction) : null;
        }

        public async Task<IEnumerable<TransactionDto>> GetByPortfolioIdAsync(string portfolioId)
        {
            var transactions = await _transactionRepository.GetByPortfolioIdAsync(portfolioId);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<TransactionDto>> GetByUserIdAsync(string userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.Select(MapToDto);
        }

        public async Task<TransactionDto> CreateAsync(CreateTransactionRequest request)
        {
            var transaction = new Transaction
            {
                PortfolioId = request.PortfolioId,
                UserId = request.UserId,
                Symbol = request.Symbol.ToUpper(),
                Quantity = request.Quantity,
                PricePaid = request.PricePaid,
                Date = request.Date ?? DateTime.UtcNow,
                TransactionType = request.TransactionType,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            return MapToDto(createdTransaction);
        }

        public async Task<TransactionDto?> UpdateAsync(string id, UpdateTransactionRequest request)
        {
            var transaction = await _transactionRepository.UpdateAsync(id, request);
            return transaction != null ? MapToDto(transaction) : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _transactionRepository.DeleteAsync(id);
        }

        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                PortfolioId = transaction.PortfolioId,
                UserId = transaction.UserId,
                Symbol = transaction.Symbol,
                Quantity = transaction.Quantity,
                PricePaid = transaction.PricePaid,
                Date = transaction.Date,
                TransactionType = transaction.TransactionType,
                Notes = transaction.Notes,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt
            };
        }
    }
}
