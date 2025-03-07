using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_currency_for_loan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Loans",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Loans");
        }
    }
}
