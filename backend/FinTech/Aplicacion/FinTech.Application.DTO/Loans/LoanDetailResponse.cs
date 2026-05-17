namespace FinTech.Application.DTO.Loans;

public class LoanDetailResponse : LoanResponse
{
    public List<PaymentScheduleDto> Schedule { get; set; } = [];
}
