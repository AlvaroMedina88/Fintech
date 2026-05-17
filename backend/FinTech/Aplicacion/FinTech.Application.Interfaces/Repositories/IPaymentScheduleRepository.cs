using FinTech.Domain.Entidades;

namespace FinTech.Application.Interfaces.Repositories;

public interface IPaymentScheduleRepository
{
    Task<IEnumerable<PaymentSchedule>> GetByLoanIdAsync(Guid loanId);
    Task AddRangeAsync(IEnumerable<PaymentSchedule> schedules);
}
