using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace SatelittiBpms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Add_UploadedFieldValueId_FieldValueFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UploadedFieldValueId",
                table: "FieldValueFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FieldValueFiles_UploadedFieldValueId",
                table: "FieldValueFiles",
                column: "UploadedFieldValueId");            

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValueFiles_FieldValues_UploadedFieldValueId",
                table: "FieldValueFiles",
                column: "UploadedFieldValueId",
                principalTable: "FieldValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);           

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldValueFiles_FieldValues_UploadedFieldValueId",
                table: "FieldValueFiles");

            migrationBuilder.DropIndex(
                name: "IX_FieldValueFiles_UploadedFieldValueId",
                table: "FieldValueFiles");

            migrationBuilder.DropColumn(
                name: "UploadedFieldValueId",
                table: "FieldValueFiles");
        }
    }
}
