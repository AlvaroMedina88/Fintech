using FinTech.Application.DTO.Transactions;
using FinTech.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTech.Api.Controladores;

[ApiController]
[Route("api/transactions")]
[Produces("application/json")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var result = await transactionService.CreateTransactionAsync(request);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? type, [FromQuery] string? status)
    {
        var result = await transactionService.GetTransactionsAsync(type, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await transactionService.GetTransactionByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
