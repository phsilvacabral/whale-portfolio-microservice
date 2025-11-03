using Microsoft.AspNetCore.Mvc;
using portfolio_service.Models;
using portfolio_service.Services;

namespace portfolio_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar transação");
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca transações por portfolio
        /// </summary>
        [HttpGet("portfolio/{portfolioId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByPortfolio(string portfolioId)
        {
            try
            {
                var transactions = await _transactionService.GetByPortfolioIdAsync(portfolioId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações para portfolio {PortfolioId}", portfolioId);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca transações por usuário
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByUser(string userId)
        {
            try
            {
                var transactions = await _transactionService.GetByUserIdAsync(userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações para usuário {UserId}", userId);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca uma transação por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(string id)
        {
            try
            {
                var transaction = await _transactionService.GetByIdAsync(id);
                if (transaction == null)
                {
                    return NotFound(new { error = "Transação não encontrada", message = "A transação solicitada não existe" });
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transação {TransactionId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza uma transação
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(string id, [FromBody] UpdateTransactionRequest request)
        {
            try
            {
                var transaction = await _transactionService.UpdateAsync(id, request);
                if (transaction == null)
                {
                    return NotFound(new { error = "Transação não encontrada", message = "A transação solicitada não existe" });
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar transação {TransactionId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Deleta uma transação
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(string id)
        {
            try
            {
                var deleted = await _transactionService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { error = "Transação não encontrada", message = "A transação solicitada não existe" });
                }

                return Ok(new { message = "Transação deletada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar transação {TransactionId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }
    }
}
