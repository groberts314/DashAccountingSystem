using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DashAccountingSystem.Data.Migrations
{
    public partial class JournalEntryAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountingPeriodId",
                table: "JournalEntry",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JournalEntryAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    JournalEntryId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    AssetTypeId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    PreviousBalance = table.Column<decimal>(nullable: true),
                    NewBalance = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryAccount_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryAccount_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryAccount_JournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_AccountingPeriodId",
                table: "JournalEntry",
                column: "AccountingPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryAccount_AccountId",
                table: "JournalEntryAccount",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryAccount_AssetTypeId",
                table: "JournalEntryAccount",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryAccount_JournalEntryId",
                table: "JournalEntryAccount",
                column: "JournalEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AccountingPeriod_AccountingPeriodId",
                table: "JournalEntry",
                column: "AccountingPeriodId",
                principalTable: "AccountingPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AccountingPeriod_AccountingPeriodId",
                table: "JournalEntry");

            migrationBuilder.DropTable(
                name: "JournalEntryAccount");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntry_AccountingPeriodId",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "AccountingPeriodId",
                table: "JournalEntry");
        }
    }
}
