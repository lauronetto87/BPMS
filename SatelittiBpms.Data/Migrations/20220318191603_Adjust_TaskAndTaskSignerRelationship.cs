using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class Adjust_TaskAndTaskSignerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_TaskSigner_Tasks_TaskId",
                "TaskSigner");

            migrationBuilder.DropIndex(
                name: "IX_TaskSigner_TaskId",
                table: "TaskSigner");

            migrationBuilder.AddForeignKey(
                "FK_TaskSigner_Tasks_TaskId",
                "TaskSigner",
                "TaskId",
                "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSigner_TaskId",
                table: "TaskSigner",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               "FK_TaskSigner_Tasks_TaskId",
               "TaskSigner");

            migrationBuilder.DropIndex(
                name: "IX_TaskSigner_TaskId",
                table: "TaskSigner");

            migrationBuilder.AddForeignKey(
               "FK_TaskSigner_Tasks_TaskId",
               "TaskSigner",
               "TaskId",
               "Tasks",
               principalColumn: "Id",
               onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSigner_TaskId",
                table: "TaskSigner",
                column: "TaskId",
                unique: true);
        }
    }
}
