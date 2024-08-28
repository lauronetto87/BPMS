using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class Add_TaskSigner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskSigner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateSendEvelope = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EnvelopeId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSigner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSigner_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaskSignerFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SignerId = table.Column<int>(type: "int", nullable: false),
                    FieldValueFileId = table.Column<int>(type: "int", nullable: false),
                    TaskSignerId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSignerFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSignerFiles_FieldValueFiles_FieldValueFileId",
                        column: x => x.FieldValueFileId,
                        principalTable: "FieldValueFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSignerFiles_TaskSigner_TaskSignerId",
                        column: x => x.TaskSignerId,
                        principalTable: "TaskSigner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSigner_TaskId",
                table: "TaskSigner",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSignerFiles_FieldValueFileId",
                table: "TaskSignerFiles",
                column: "FieldValueFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSignerFiles_TaskSignerId",
                table: "TaskSignerFiles",
                column: "TaskSignerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskSignerFiles");

            migrationBuilder.DropTable(
                name: "TaskSigner");
        }
    }
}
