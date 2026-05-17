using FinTech.Application.Interfaces.Repositories;
using FinTech.Domain.Entidades;
using FinTech.Domain.Enumeraciones;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistencia.Repositorios;

public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository
{
    public async Task<IEnumerable<Transaction>> GetAllAsync(string? type = null, string? status = null)
    {
        var query = context.Transactions.AsQueryable();

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<TransactionType>(type, true, out var transactionType))
            query = query.Where(t => t.Type == transactionType);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TransactionStatus>(status, true, out var transactionStatus))
            query = query.Where(t => t.Status == transactionStatus);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(Guid id) =>
        await context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Transaction?> GetByIdempotencyKeyAsync(string key) =>
        await context.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == key);

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }
}
