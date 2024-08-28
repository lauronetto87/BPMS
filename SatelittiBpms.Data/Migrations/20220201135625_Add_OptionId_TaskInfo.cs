using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace SatelittiBpms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Add_OptionId_TaskInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OptionId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OptionId",
                table: "Tasks",
                column: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_ActivityUsersOptions_OptionId",
                table: "Tasks",
                column: "OptionId",
                principalTable: "ActivityUsersOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_ActivityUsersOptions_OptionId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OptionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OptionId",
                table: "Tasks");
        }
    }
}
