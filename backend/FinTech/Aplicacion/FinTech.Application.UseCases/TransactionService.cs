using FinTech.Application.DTO.Transactions;
using FinTech.Application.Interfaces.Repositories;
using FinTech.Application.Interfaces.Services;
using FinTech.Domain.Entidades;
using FinTech.Domain.Enumeraciones;
using Microsoft.Extensions.Logging;

namespace FinTech.Application.UseCases;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ILogger<TransactionService> logger) : ITransactionService
{
    public async Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request)
    {
        // Idempotencia: si ya existe con el mismo key, retorna la original
        var existing = await transactionRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey);
        if (existing is not null)
        {
            logger.LogWarning("Transaccion duplicada. LLave: {Key}. la key orignal: ", request.IdempotencyKey);
            return MapToResponse(existing);
        }

        if (!Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
            throw new ArgumentException($"Tipo de transaccion invalido: {request.Type}");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = request.IdempotencyKey,
            Type = transactionType,
            Amount = request.Amount,
            Status = TransactionStatus.Completed,
            LoanId = request.LoanId,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await transactionRepository.AddAsync(transaction);
        logger.LogInformation("Transaccion {Id} creada. Tipo: {Type}, Monto: {Amount}.", transaction.Id, transaction.Type, transaction.Amount);

        return MapToResponse(transaction);
    }

    public async Task<IEnumerable<TransactionResponse>> GetTransactionsAsync(string? type = null, string? status = null)
    {
        var transactions = await transactionRepository.GetAllAsync(type, status);
        return transactions.Select(MapToResponse);
    }

    public async Task<TransactionResponse?> GetTransactionByIdAsync(Guid id)
    {
        var transaction = await transactionRepository.GetByIdAsync(id);
        return transaction is null ? null : MapToResponse(transaction);
    }

    private static TransactionResponse MapToResponse(Transaction t) => new()
    {
        Id = t.Id,
        IdempotencyKey = t.IdempotencyKey,
        Type = t.Type.ToString(),
        Amount = t.Amount,
        Status = t.Status.ToString(),
        LoanId = t.LoanId,
        Description = t.Description,
        CreatedAt = t.CreatedAt
    };
}
