using FinTech.Application.DTO.Loans;
using FinTech.Application.Interfaces.Repositories;
using FinTech.Application.Interfaces.Services;
using FinTech.Domain;
using FinTech.Domain.Entidades;
using FinTech.Domain.Enumeraciones;
using Microsoft.Extensions.Logging;

namespace FinTech.Application.UseCases;

public class LoanService(
    ILoanRepository loanRepository,
    IPaymentScheduleRepository scheduleRepository,
    ITransactionRepository transactionRepository,
    ILogger<LoanService> logger) : ILoanService
{
    public Task<SimulateLoanResponse> SimulateLoanAsync(SimulateLoanRequest request)
    {
        ValidateAmount(request.Amount);
        ValidateTerm(request.Term);

        var tem = FinancialCalculator.CalculateTEM(request.Tea);
        var monthlyPayment = FinancialCalculator.CalculateFixedPayment(request.Amount, request.Term, tem);
        var schedule = FinancialCalculator.GenerateSchedule(request.Amount, request.Term, request.Tea, DateTime.UtcNow);

        return Task.FromResult(new SimulateLoanResponse
        {
            Amount = request.Amount,
            Term = request.Term,
            Tea = request.Tea,
            Tem = Math.Round(tem, 6),
            MonthlyPayment = monthlyPayment,
            Schedule = schedule.Select(MapToDto).ToList()
        });
    }

    public async Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request)
    {
        ValidateAmount(request.Amount);
        ValidateTerm(request.Term);

        var activeLoans = await loanRepository.GetActiveByUserIdAsync(request.UserId);
        var activeList = activeLoans.ToList();

        if (activeList.Count >= 3)
        {
            throw new InvalidOperationException("El cliente ya tiene 3 préstamos activos. No se puede solicitar mas.");
        }

        var tem = FinancialCalculator.CalculateTEM(request.Tea);
        var monthlyPayment = FinancialCalculator.CalculateFixedPayment(request.Amount, request.Term, tem);

        var totalMonthlyPayments = activeList.Sum(l => l.MonthlyPayment) + monthlyPayment;
        if (request.MonthlyIncome > 0 && totalMonthlyPayments > request.MonthlyIncome * 0.4m)
        {
            throw new InvalidOperationException($"La suma de cuotas ({totalMonthlyPayments:C}) supera el 40% del ingreso mensual ({request.MonthlyIncome * 0.4m:C}).");
        }

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = request.Amount,
            Term = request.Term,
            InterestRate = request.Tea,
            LoanType = LoanType.Fixed,
            Status = LoanStatus.Pending,
            MonthlyPayment = monthlyPayment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await loanRepository.AddAsync(loan);

        var scheduleItems = FinancialCalculator.GenerateSchedule(request.Amount, request.Term, request.Tea, DateTime.UtcNow);
        var scheduleEntities = scheduleItems.Select(s => new PaymentSchedule
        {
            Id = Guid.NewGuid(),
            LoanId = loan.Id,
            PaymentNumber = s.PaymentNumber,
            DueDate = s.DueDate,
            TotalPayment = s.TotalPayment,
            Principal = s.Principal,
            Interest = s.Interest,
            RemainingBalance = s.RemainingBalance,
            Status = PaymentStatus.Pending
        });

        await scheduleRepository.AddRangeAsync(scheduleEntities);

        if (request.Amount < 10000m && activeList.Count < 2)
        {
            logger.LogInformation("Se Auto aprobando préstamo {LoanId} por scoring automático.", loan.Id);
            return await ApproveLoanAsync(loan.Id);
        }

        logger.LogInformation("Préstamo {LoanId} creado en estado Pending.", loan.Id);
        return MapToResponse(loan);
    }

    public async Task<IEnumerable<LoanResponse>> GetLoansAsync(string? userId = null)
    {
        var loans = await loanRepository.GetAllAsync(userId);
        return loans.Select(MapToResponse);
    }

    public async Task<LoanDetailResponse?> GetLoanByIdAsync(Guid id)
    {
        var loan = await loanRepository.GetByIdAsync(id);
        if (loan is null) return null;

        var schedule = await scheduleRepository.GetByLoanIdAsync(id);
        var response = new LoanDetailResponse
        {
            Id = loan.Id,
            UserId = loan.UserId,
            Amount = loan.Amount,
            Term = loan.Term,
            InterestRate = loan.InterestRate,
            LoanType = loan.LoanType.ToString(),
            Status = loan.Status.ToString(),
            MonthlyPayment = loan.MonthlyPayment,
            CreatedAt = loan.CreatedAt,
            Schedule = schedule.Select(MapToDto).ToList()
        };

        return response;
    }

    public async Task<IEnumerable<PaymentScheduleDto>> GetScheduleAsync(Guid loanId)
    {
        var schedule = await scheduleRepository.GetByLoanIdAsync(loanId);
        return schedule.Select(MapToDto);
    }

    public async Task<LoanResponse> ApproveLoanAsync(Guid id)
    {
        var loan = await loanRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Préstamo {id} no encontrado");

        if (loan.Status != LoanStatus.Pending)
            throw new InvalidOperationException($"Solo se puede aprobar préstamos en estado Pending. Estado actual: {loan.Status}.");

        loan.Status = LoanStatus.Active;
        await loanRepository.UpdateAsync(loan);

        await transactionRepository.AddAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = $"disbursement-{loan.Id}",
            Type = TransactionType.Disbursement,
            Amount = loan.Amount,
            Status = TransactionStatus.Completed,
            LoanId = loan.Id,
            Description = $"Desembolso préstamo aprobado",
            CreatedAt = DateTime.UtcNow
        });

        logger.LogInformation("Prestamo {LoanId} aprobado y desembolsado", id);
        return MapToResponse(loan);
    }

    public async Task<LoanResponse> RejectLoanAsync(Guid id)
    {
        var loan = await loanRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Préstamo {id} no encontrado");

        if (loan.Status != LoanStatus.Pending)
            throw new InvalidOperationException($"Solo se pueden rechazar préstamos en estado Pending. Estado actual: {loan.Status}.");

        loan.Status = LoanStatus.Rejected;
        await loanRepository.UpdateAsync(loan);

        logger.LogInformation("Préstamo {LoanId} rechazado.", id);
        return MapToResponse(loan);
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 500 || amount > 50000)
            throw new ArgumentException("El monto debe estar entre $500 y $50,000");
    }

    private static void ValidateTerm(int term)
    {
        if (term < 6 || term > 60)
            throw new ArgumentException("El plazo debe estar entre 6 y 60 meses.");
    }

    private static LoanResponse MapToResponse(Loan loan) => new()
    {
        Id = loan.Id,
        UserId = loan.UserId,
        Amount = loan.Amount,
        Term = loan.Term,
        InterestRate = loan.InterestRate,
        LoanType = loan.LoanType.ToString(),
        Status = loan.Status.ToString(),
        MonthlyPayment = loan.MonthlyPayment,
        CreatedAt = loan.CreatedAt
    };

    private static PaymentScheduleDto MapToDto(PaymentSchedule s) => new()
    {
        PaymentNumber = s.PaymentNumber,
        DueDate = s.DueDate,
        TotalPayment = s.TotalPayment,
        Principal = s.Principal,
        Interest = s.Interest,
        RemainingBalance = s.RemainingBalance,
        Status = s.Status.ToString()
    };

    private static PaymentScheduleDto MapToDto(PaymentScheduleItem s) => new()
    {
        PaymentNumber = s.PaymentNumber,
        DueDate = s.DueDate,
        TotalPayment = s.TotalPayment,
        Principal = s.Principal,
        Interest = s.Interest,
        RemainingBalance = s.RemainingBalance,
        Status = "Pending"
    };
}
