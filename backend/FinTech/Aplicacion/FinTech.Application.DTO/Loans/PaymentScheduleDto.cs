namespace FinTech.Application.DTO.Loans;

public class PaymentScheduleDto
{
    public int PaymentNumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalPayment { get; set; }
    public decimal Principal { get; set; }
    public decimal Interest { get; set; }
    public decimal RemainingBalance { get; set; }
    public string Status { get; set; } = "Pending";
}
