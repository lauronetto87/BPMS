using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace SatelittiBpms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Add_DescriptionFlow_ProcessVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionFlow",
                table: "ProcessVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionFlow",
                table: "ProcessVersions");
        }
    }
}
