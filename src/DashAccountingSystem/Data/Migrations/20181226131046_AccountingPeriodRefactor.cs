using Microsoft.EntityFrameworkCore.Migrations;

namespace DashAccountingSystem.Data.Migrations
{
    public partial class AccountingPeriodRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "AccountingPeriodType",
                table: "Tenant",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "PeriodType",
                table: "AccountingPeriod",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "AccountingPeriodClosingBalance",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    AccountingPeriodId = table.Column<int>(nullable: false),
                    ClosingBalance = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingPeriodClosingBalance", x => new { x.AccountId, x.AccountingPeriodId });
                    table.ForeignKey(
                        name: "FK_AccountingPeriodClosingBalance_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountingPeriodClosingBalance_AccountingPeriod_AccountingP~",
                        column: x => x.AccountingPeriodId,
                        principalTable: "AccountingPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPeriodClosingBalance_AccountingPeriodId",
                table: "AccountingPeriodClosingBalance",
                column: "AccountingPeriodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingPeriodClosingBalance");

            migrationBuilder.DropColumn(
                name: "AccountingPeriodType",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "PeriodType",
                table: "AccountingPeriod");
        }
    }
}
