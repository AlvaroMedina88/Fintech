using FinTech.Application.DTO.Transactions;
using FinTech.Application.Interfaces.Repositories;
using FinTech.Application.UseCases;
using FinTech.Domain.Entidades;
using FinTech.Domain.Enumeraciones;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace FinTech.Tests;

public class TransactionIdempotencyTests
{
    [Fact]
    public async Task CreateTransaction_WithDuplicateIdempotencyKey_ReturnsSameTransaction()
    {
        const string idempotencyKey = "test-key-abc123";
        var existingId = Guid.NewGuid();

        var existingTransaction = new Transaction
        {
            Id = existingId,
            IdempotencyKey = idempotencyKey,
            Type = TransactionType.Payment,
            Amount = 200m,
            Status = TransactionStatus.Completed,
            CreatedAt = DateTime.UtcNow
        };

        var repoMock = new Mock<ITransactionRepository>();
        repoMock.Setup(r => r.GetByIdempotencyKeyAsync(idempotencyKey))
                .ReturnsAsync(existingTransaction);

        var service = new TransactionService(repoMock.Object, NullLogger<TransactionService>.Instance);

        var request = new CreateTransactionRequest
        {
            IdempotencyKey = idempotencyKey,
            Type = "Payment",
            Amount = 999m  
        };

        var result = await service.CreateTransactionAsync(request);

        Assert.Equal(existingId, result.Id);
        Assert.Equal(200m, result.Amount);
        repoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Never);
    }
}
