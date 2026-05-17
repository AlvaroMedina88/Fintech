using FinTech.Application.DTO.Loans;

namespace FinTech.Application.Interfaces.Services;

public interface ILoanService
{
    Task<SimulateLoanResponse> SimulateLoanAsync(SimulateLoanRequest request);
    Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetLoansAsync(string? userId = null);
    Task<LoanDetailResponse?> GetLoanByIdAsync(Guid id);
    Task<IEnumerable<PaymentScheduleDto>> GetScheduleAsync(Guid loanId);
    Task<LoanResponse> ApproveLoanAsync(Guid id);
    Task<LoanResponse> RejectLoanAsync(Guid id);
}
