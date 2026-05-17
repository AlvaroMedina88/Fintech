namespace FinTech.Application.DTO.Loans;

public class SimulateLoanResponse
{
    public decimal Amount { get; set; }
    public int Term { get; set; }
    public decimal Tea { get; set; }
    public decimal Tem { get; set; }
    public decimal MonthlyPayment { get; set; }
    public List<PaymentScheduleDto> Schedule { get; set; } = [];
}
