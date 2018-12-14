using Microsoft.EntityFrameworkCore.Migrations;

namespace DashAccountingSystem.Data.Migrations
{
    public partial class JournalEntryCreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AspNetUsers_EnteredById",
                table: "JournalEntry");

            migrationBuilder.RenameColumn(
                name: "EnteredById",
                table: "JournalEntry",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntry_EnteredById",
                table: "JournalEntry",
                newName: "IX_JournalEntry_CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AspNetUsers_CreatedById",
                table: "JournalEntry",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AspNetUsers_CreatedById",
                table: "JournalEntry");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "JournalEntry",
                newName: "EnteredById");

            migrationBuilder.RenameIndex(
                name: "IX_JournalEntry_CreatedById",
                table: "JournalEntry",
                newName: "IX_JournalEntry_EnteredById");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AspNetUsers_EnteredById",
                table: "JournalEntry",
                column: "EnteredById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
