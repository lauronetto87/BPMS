using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class Add_TaskSignerStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TaskSigner",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TaskSigner");
        }
    }
}
