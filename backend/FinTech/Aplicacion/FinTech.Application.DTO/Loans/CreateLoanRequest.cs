namespace FinTech.Application.DTO.Loans;

public class CreateLoanRequest
{
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal Tea { get; set; } = 0.24m;
}
