using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SplitMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixExpenseRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_PaidById",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaidById",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaidById",
                table: "Expenses");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaidByUserId",
                table: "Expenses",
                column: "PaidByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_PaidByUserId",
                table: "Expenses",
                column: "PaidByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_PaidByUserId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaidByUserId",
                table: "Expenses");

            migrationBuilder.AddColumn<Guid>(
                name: "PaidById",
                table: "Expenses",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaidById",
                table: "Expenses",
                column: "PaidById");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_PaidById",
                table: "Expenses",
                column: "PaidById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
