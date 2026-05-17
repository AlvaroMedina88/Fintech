using FinTech.Application.Interfaces.Repositories;
using FinTech.Domain.Entidades;
using FinTech.Domain.Enumeraciones;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistencia.Repositorios;

public class LoanRepository(ApplicationDbContext context) : ILoanRepository
{
    public async Task<IEnumerable<Loan>> GetAllAsync(string? userId = null)
    {
        var query = context.Loans.AsQueryable();
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(l => l.UserId == userId);
        return await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(Guid id) =>
        await context.Loans.Include(l => l.PaymentSchedules).FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Loan>> GetActiveByUserIdAsync(string userId) =>
        await context.Loans
            .Where(l => l.UserId == userId && (l.Status == LoanStatus.Active || l.Status == LoanStatus.Approved))
            .ToListAsync();

    public async Task<Loan> AddAsync(Loan loan)
    {
        context.Loans.Add(loan);
        await context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan> UpdateAsync(Loan loan)
    {
        loan.UpdatedAt = DateTime.UtcNow;
        context.Loans.Update(loan);
        await context.SaveChangesAsync();
        return loan;
    }
}
