using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DashAccountingSystem.Data.Migrations
{
    public partial class AddlAuditFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "JournalEntry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "JournalEntry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Account",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Account",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_UpdatedById",
                table: "JournalEntry",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CreatedById",
                table: "Account",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Account_UpdatedById",
                table: "Account",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_AspNetUsers_CreatedById",
                table: "Account",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_AspNetUsers_UpdatedById",
                table: "Account",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntry_AspNetUsers_UpdatedById",
                table: "JournalEntry",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_AspNetUsers_CreatedById",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_AspNetUsers_UpdatedById",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntry_AspNetUsers_UpdatedById",
                table: "JournalEntry");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntry_UpdatedById",
                table: "JournalEntry");

            migrationBuilder.DropIndex(
                name: "IX_Account_CreatedById",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_UpdatedById",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Account");
        }
    }
}
