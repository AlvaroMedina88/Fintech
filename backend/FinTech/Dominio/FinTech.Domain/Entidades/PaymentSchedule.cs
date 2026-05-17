using FinTech.Domain.Enumeraciones;

namespace FinTech.Domain.Entidades;

public class PaymentSchedule
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public int PaymentNumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalPayment { get; set; }
    public decimal Principal { get; set; }
    public decimal Interest { get; set; }
    public decimal RemainingBalance { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Loan Loan { get; set; } = null!;
}
