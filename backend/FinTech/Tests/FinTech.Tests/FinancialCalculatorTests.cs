using FinTech.Domain;
using Xunit;

namespace FinTech.Tests;

public class FinancialCalculatorTests
{
    [Fact]
    public void CalculateFixedPayment_ReturnsCorrectAmount()
    {
        // Préstamo: $5000, 12 meses, TEA 24%
        var tem = FinancialCalculator.CalculateTEM(0.24m);
        var payment = FinancialCalculator.CalculateFixedPayment(5000m, 12, tem);

        // Valor esperado calculado manualmente con sistema francés
        Assert.True(payment > 460m && payment < 480m, $"Cuota esperada entre $460 y $480, obtenida: {payment}");
    }

    [Fact]
    public void GenerateSchedule_ReturnsCorrectNumberOfPayments()
    {
        var schedule = FinancialCalculator.GenerateSchedule(5000m, 12, 0.24m, DateTime.UtcNow);

        Assert.Equal(12, schedule.Count);
    }

    [Fact]
    public void GenerateSchedule_LastBalanceIsZero()
    {
        var schedule = FinancialCalculator.GenerateSchedule(10000m, 24, 0.24m, DateTime.UtcNow);

        Assert.Equal(0m, schedule.Last().RemainingBalance);
    }

    [Fact]
    public void CalculateTEM_FromTEA_ReturnsCorrectValue()
    {
        var tem = FinancialCalculator.CalculateTEM(0.24m);
        Assert.True(tem > 0.018m && tem < 0.019m, $"TEM esperada ~0.0181, obtenida: {tem}");
    }
}
