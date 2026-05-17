using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinTech.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Term = table.Column<int>(type: "integer", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    LoanType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentNumber = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Principal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "Id", "Amount", "CreatedAt", "InterestRate", "LoanType", "MonthlyIncome", "MonthlyPayment", "Status", "Term", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 5000m, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 0.24m, "Fixed", 3000m, 467.26m, "Active", 12, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "user-001" });

            migrationBuilder.InsertData(
                table: "PaymentSchedules",
                columns: new[] { "Id", "DueDate", "Interest", "LoanId", "PaymentNumber", "Principal", "RemainingBalance", "Status", "TotalPayment" },
                values: new object[,]
                {
                    { new Guid("c0000001-0000-0000-0000-000000000001"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 90.44m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 1, 376.82m, 4623.18m, "Paid", 467.26m },
                    { new Guid("c0000002-0000-0000-0000-000000000001"), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), 83.62m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 2, 383.64m, 4239.54m, "Pending", 467.26m },
                    { new Guid("c0000003-0000-0000-0000-000000000001"), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), 76.68m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 3, 390.58m, 3848.96m, "Pending", 467.26m },
                    { new Guid("c0000004-0000-0000-0000-000000000001"), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), 69.62m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 4, 397.64m, 3451.32m, "Pending", 467.26m },
                    { new Guid("c0000005-0000-0000-0000-000000000001"), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 62.43m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 5, 404.83m, 3046.49m, "Pending", 467.26m },
                    { new Guid("c0000006-0000-0000-0000-000000000001"), new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc), 55.10m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 6, 412.16m, 2634.33m, "Pending", 467.26m },
                    { new Guid("c0000007-0000-0000-0000-000000000001"), new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), 47.65m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 7, 419.61m, 2214.72m, "Pending", 467.26m },
                    { new Guid("c0000008-0000-0000-0000-000000000001"), new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Utc), 40.06m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 8, 427.20m, 1787.52m, "Pending", 467.26m },
                    { new Guid("c0000009-0000-0000-0000-000000000001"), new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), 32.33m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 9, 434.93m, 1352.59m, "Pending", 467.26m },
                    { new Guid("c0000010-0000-0000-0000-000000000001"), new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Utc), 24.47m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 10, 442.79m, 909.80m, "Pending", 467.26m },
                    { new Guid("c0000011-0000-0000-0000-000000000001"), new DateTime(2026, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), 16.46m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 11, 450.80m, 459.00m, "Pending", 467.26m },
                    { new Guid("c0000012-0000-0000-0000-000000000001"), new DateTime(2027, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 8.30m, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 12, 459.00m, 0.00m, "Pending", 467.30m }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "CreatedAt", "Description", "IdempotencyKey", "LoanId", "Status", "Type" },
                values: new object[] { new Guid("d1e2f3a4-b5c6-7890-abcd-ef1234567890"), 5000m, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Desembolso préstamo aprobado", "disbursement-a1b2c3d4", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "Completed", "Disbursement" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_LoanId",
                table: "PaymentSchedules",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdempotencyKey",
                table: "Transactions",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanId",
                table: "Transactions",
                column: "LoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentSchedules");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Loans");
        }
    }
}
