using FinTech.Application.DTO.Transactions;

namespace FinTech.Application.Interfaces.Services;

public interface ITransactionService
{
    Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request);

    Task<IEnumerable<TransactionResponse>> GetTransactionsAsync(string? type = null, string? status = null);
    Task<TransactionResponse?> GetTransactionByIdAsync(Guid id);
}
