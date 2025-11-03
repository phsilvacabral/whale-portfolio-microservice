using Microsoft.AspNetCore.Mvc;
using portfolio_service.Models;
using portfolio_service.Services;

namespace portfolio_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioService portfolioService, ILogger<PortfolioController> logger)
        {
            _portfolioService = portfolioService;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo portfolio
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PortfolioDto>> CreatePortfolio([FromBody] CreatePortfolioRequest request)
        {
            try
            {
                var portfolio = await _portfolioService.CreateAsync(request);
                return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.Id }, portfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar portfolio");
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca portfolios por usuário
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetPortfolios([FromQuery] string userId)
        {
            try
            {
                var portfolios = await _portfolioService.GetByUserIdAsync(userId);
                return Ok(portfolios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar portfolios para usuário {UserId}", userId);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca um portfolio por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio(string id)
        {
            try
            {
                var portfolio = await _portfolioService.GetByIdAsync(id);
                if (portfolio == null)
                {
                    return NotFound(new { error = "Portfolio não encontrado", message = "O portfolio solicitado não existe" });
                }

                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar portfolio {PortfolioId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza um portfolio
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PortfolioDto>> UpdatePortfolio(string id, [FromBody] UpdatePortfolioRequest request)
        {
            try
            {
                var portfolio = await _portfolioService.UpdateAsync(id, request);
                if (portfolio == null)
                {
                    return NotFound(new { error = "Portfolio não encontrado", message = "O portfolio solicitado não existe" });
                }

                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar portfolio {PortfolioId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Deleta um portfolio (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePortfolio(string id)
        {
            try
            {
                var deleted = await _portfolioService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { error = "Portfolio não encontrado", message = "O portfolio solicitado não existe" });
                }

                return Ok(new { message = "Portfolio deletado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar portfolio {PortfolioId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }
    }
}
