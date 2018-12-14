using Microsoft.EntityFrameworkCore.Migrations;

namespace DashAccountingSystem.Data.Migrations
{
    public partial class JournalEntryNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "JournalEntry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "JournalEntry");
        }
    }
}
