using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class Add_FileKey_FieldValueFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileKey",
                table: "FieldValueFiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileKey",
                table: "FieldValueFiles");
        }
    }
}
