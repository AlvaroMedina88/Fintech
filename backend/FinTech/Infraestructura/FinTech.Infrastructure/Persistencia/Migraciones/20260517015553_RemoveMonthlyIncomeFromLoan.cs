using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTech.Infrastructure.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class RemoveMonthlyIncomeFromLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyIncome",
                table: "Loans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyIncome",
                table: "Loans",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                column: "MonthlyIncome",
                value: 3000m);
        }
    }
}
