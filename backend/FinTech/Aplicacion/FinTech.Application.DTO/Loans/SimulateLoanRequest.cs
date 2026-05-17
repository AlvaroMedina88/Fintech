namespace FinTech.Application.DTO.Loans;

public class SimulateLoanRequest
{
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public decimal Tea { get; set; } = 0.24m;
}
