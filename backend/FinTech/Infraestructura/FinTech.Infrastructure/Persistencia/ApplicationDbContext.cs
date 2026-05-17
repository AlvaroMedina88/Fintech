using FinTech.Domain.Entidades;
using FinTech.Domain.Enumeraciones;
using FinTech.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistencia;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(18,2)");
            entity.Property(e => e.InterestRate).HasColumnType("decimal(18,4)");
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.LoanType).HasConversion<string>();
            entity.HasMany(e => e.PaymentSchedules).WithOne(e => e.Loan).HasForeignKey(e => e.LoanId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Transactions).WithOne(e => e.Loan).HasForeignKey(e => e.LoanId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PaymentSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalPayment).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Principal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Interest).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RemainingBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IdempotencyKey).IsUnique();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Type).HasConversion<string>();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var loanId1 = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        var loan1 = new Loan
        {
            Id = loanId1,
            UserId = "user-001",
            Amount = 5000m,
            Term = 12,
            InterestRate = 0.24m,
            LoanType = LoanType.Fixed,
            Status = LoanStatus.Active,
            MonthlyPayment = 0m,
            CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        };

        var tem = FinancialCalculator.CalculateTEM(0.24m);
        loan1.MonthlyPayment = FinancialCalculator.CalculateFixedPayment(5000m, 12, tem);

        modelBuilder.Entity<Loan>().HasData(loan1);

        var scheduleGuids = new Guid[]
        {
            new("c0000001-0000-0000-0000-000000000001"),
            new("c0000002-0000-0000-0000-000000000001"),
            new("c0000003-0000-0000-0000-000000000001"),
            new("c0000004-0000-0000-0000-000000000001"),
            new("c0000005-0000-0000-0000-000000000001"),
            new("c0000006-0000-0000-0000-000000000001"),
            new("c0000007-0000-0000-0000-000000000001"),
            new("c0000008-0000-0000-0000-000000000001"),
            new("c0000009-0000-0000-0000-000000000001"),
            new("c0000010-0000-0000-0000-000000000001"),
            new("c0000011-0000-0000-0000-000000000001"),
            new("c0000012-0000-0000-0000-000000000001"),
        };

        var schedule = FinancialCalculator.GenerateSchedule(5000m, 12, 0.24m, new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc));
        var scheduleEntities = schedule.Select((s, idx) => new PaymentSchedule
        {
            Id = scheduleGuids[idx],
            LoanId = loanId1,
            PaymentNumber = s.PaymentNumber,
            DueDate = DateTime.SpecifyKind(s.DueDate, DateTimeKind.Utc),
            TotalPayment = s.TotalPayment,
            Principal = s.Principal,
            Interest = s.Interest,
            RemainingBalance = s.RemainingBalance,
            Status = idx == 0 ? PaymentStatus.Paid : PaymentStatus.Pending
        }).ToList();

        modelBuilder.Entity<PaymentSchedule>().HasData(scheduleEntities);

        modelBuilder.Entity<Transaction>().HasData(new Transaction
        {
            Id = new Guid("d1e2f3a4-b5c6-7890-abcd-ef1234567890"),
            IdempotencyKey = "disbursement-a1b2c3d4",
            Type = TransactionType.Disbursement,
            Amount = 5000m,
            Status = TransactionStatus.Completed,
            LoanId = loanId1,
            Description = "Desembolso préstamo aprobado",
            CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
