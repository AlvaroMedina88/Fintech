namespace FinTech.Domain;

public static class FinancialCalculator
{
    public static decimal CalculateTEM(decimal tea)
    {
        return (decimal)(Math.Pow((double)(1 + tea), 1.0 / 12) - 1);
    }

    public static decimal CalculateFixedPayment(decimal amount, int term, decimal tem)
    {
        if (tem == 0) return Math.Round(amount / term, 2);
        double r = (double)tem;
        double n = term;
        double factor = Math.Pow(1 + r, n);
        return Math.Round((decimal)(r * factor / (factor - 1)) * amount, 2);
    }

    public static List<PaymentScheduleItem> GenerateSchedule(decimal amount, int term, decimal tea, DateTime startDate)
    {
        var tem = CalculateTEM(tea);
        var monthlyPayment = CalculateFixedPayment(amount, term, tem);
        var schedule = new List<PaymentScheduleItem>();
        var balance = amount;

        for (int i = 1; i <= term; i++)
        {
            var interest = Math.Round(balance * tem, 2);
            var principal = Math.Round(monthlyPayment - interest, 2);

            if (i == term)
                principal = balance;

            balance = Math.Round(balance - principal, 2);
            if (balance < 0) balance = 0;

            var dueDate = startDate.AddMonths(i);
            if (dueDate.Day != startDate.Day)
            {
                var daysInMonth = DateTime.DaysInMonth(dueDate.Year, dueDate.Month);
                dueDate = new DateTime(dueDate.Year, dueDate.Month, Math.Min(startDate.Day, daysInMonth));
            }

            schedule.Add(new PaymentScheduleItem
            {
                PaymentNumber = i,
                DueDate = dueDate,
                TotalPayment = Math.Round(principal + interest, 2),
                Principal = principal,
                Interest = interest,
                RemainingBalance = balance
            });
        }

        return schedule;
    }
}

public record PaymentScheduleItem
{
    public int PaymentNumber { get; init; }
    public DateTime DueDate { get; init; }
    public decimal TotalPayment { get; init; }
    public decimal Principal { get; init; }
    public decimal Interest { get; init; }
    public decimal RemainingBalance { get; init; }
}
