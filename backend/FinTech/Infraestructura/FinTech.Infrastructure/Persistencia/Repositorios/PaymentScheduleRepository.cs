using FinTech.Application.Interfaces.Repositories;
using FinTech.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistencia.Repositorios;

public class PaymentScheduleRepository(ApplicationDbContext context) : IPaymentScheduleRepository
{
    public async Task<IEnumerable<PaymentSchedule>> GetByLoanIdAsync(Guid loanId) =>
        await context.PaymentSchedules
            .Where(p => p.LoanId == loanId)
            .OrderBy(p => p.PaymentNumber)
            .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<PaymentSchedule> schedules)
    {
        context.PaymentSchedules.AddRange(schedules);
        await context.SaveChangesAsync();
    }
}
