using FinTech.Domain.Enumeraciones;

namespace FinTech.Domain.Entidades;

public class Loan
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public decimal InterestRate { get; set; } = 0.24m;
    public LoanType LoanType { get; set; } = LoanType.Fixed;
    public LoanStatus Status { get; set; } = LoanStatus.Pending;
    public decimal MonthlyPayment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PaymentSchedule> PaymentSchedules { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
}
