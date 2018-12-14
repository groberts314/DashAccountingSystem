using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DashAccountingSystem.Data.Migrations
{
    public partial class AccountAndJournalEntryUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CheckNumber",
                table: "JournalEntry",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "JournalEntry",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CanceledById",
                table: "JournalEntry",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "JournalEntry",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "PendingCredits",
                table: "Account",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PendingDebits",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_CanceledById",
                table: "JournalEntry",
                column: "CanceledById");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AspNetUsers_CanceledById",
                table: "JournalEntry",
                column: "CanceledById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AspNetUsers_CanceledById",
                table: "JournalEntry");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntry_CanceledById",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "CanceledById",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "PendingCredits",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "PendingDebits",
                table: "Account");

            migrationBuilder.AlterColumn<int>(
                name: "CheckNumber",
                table: "JournalEntry",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
