using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace SatelittiBpms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Add_PersonId_ActivityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "ActivityUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityUsers_PersonId",
                table: "ActivityUsers",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityUsers_Users_PersonId",
                table: "ActivityUsers",
                column: "PersonId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityUsers_Users_PersonId",
                table: "ActivityUsers");

            migrationBuilder.DropIndex(
                name: "IX_ActivityUsers_PersonId",
                table: "ActivityUsers");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "ActivityUsers");
        }
    }
}
