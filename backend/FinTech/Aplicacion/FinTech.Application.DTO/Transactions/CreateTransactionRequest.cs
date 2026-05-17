namespace FinTech.Application.DTO.Transactions;

public class CreateTransactionRequest
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid? LoanId { get; set; }
    public string Description { get; set; } = string.Empty;
}
