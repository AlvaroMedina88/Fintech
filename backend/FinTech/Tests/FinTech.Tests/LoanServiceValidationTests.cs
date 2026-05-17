using FinTech.Application.DTO.Loans;
using FinTech.Application.Interfaces.Repositories;
using FinTech.Application.UseCases;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace FinTech.Tests;

public class LoanServiceValidationTests
{
    private readonly LoanService _service;

    public LoanServiceValidationTests()
    {
        var loanRepo = new Mock<ILoanRepository>();
        var scheduleRepo = new Mock<IPaymentScheduleRepository>();
        var transactionRepo = new Mock<ITransactionRepository>();
        var logger = NullLogger<LoanService>.Instance;

        _service = new LoanService(loanRepo.Object, scheduleRepo.Object, transactionRepo.Object, logger);
    }

    [Fact]
    public async Task SimulateLoan_WithAmountBelowMinimum_ThrowsArgumentException()
    {
        var request = new SimulateLoanRequest { Amount = 100m, Term = 12 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulateLoanAsync(request));
    }

    [Fact]
    public async Task SimulateLoan_WithAmountAboveMaximum_ThrowsArgumentException()
    {
        var request = new SimulateLoanRequest { Amount = 100000m, Term = 12 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulateLoanAsync(request));
    }

    [Fact]
    public async Task SimulateLoan_WithTermBelowMinimum_ThrowsArgumentException()
    {
        var request = new SimulateLoanRequest { Amount = 5000m, Term = 3 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulateLoanAsync(request));
    }

    [Fact]
    public async Task SimulateLoan_WithTermAboveMaximum_ThrowsArgumentException()
    {
        var request = new SimulateLoanRequest { Amount = 5000m, Term = 72 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulateLoanAsync(request));
    }
}
