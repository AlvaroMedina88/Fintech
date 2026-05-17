using FinTech.Application.DTO.Loans;
using FinTech.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTech.Api.Controladores;

[ApiController]
[Route("api/loans")]
[Produces("application/json")]
public class LoansController(ILoanService loanService) : ControllerBase
{
    [HttpPost("simulate")]
    [ProducesResponseType(typeof(SimulateLoanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Simulate([FromBody] SimulateLoanRequest request)
    {
        var result = await loanService.SimulateLoanAsync(request);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LoanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateLoanRequest request)
    {
        var result = await loanService.CreateLoanAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LoanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? userId)
    {
        var result = await loanService.GetLoansAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await loanService.GetLoanByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/schedule")]
    [ProducesResponseType(typeof(IEnumerable<PaymentScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        var result = await loanService.GetScheduleAsync(id);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(typeof(LoanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await loanService.ApproveLoanAsync(id);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/reject")]
    [ProducesResponseType(typeof(LoanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await loanService.RejectLoanAsync(id);
        return Ok(result);
    }
}
