using FinTech.Domain.Entidades;

namespace FinTech.Application.Interfaces.Repositories;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync(string? userId = null);
    Task<Loan?> GetByIdAsync(Guid id);
    Task<IEnumerable<Loan>> GetActiveByUserIdAsync(string userId);
    Task<Loan> AddAsync(Loan loan);
    Task<Loan> UpdateAsync(Loan loan);
}
