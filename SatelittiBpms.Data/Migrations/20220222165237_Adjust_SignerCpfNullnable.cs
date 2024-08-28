using Microsoft.EntityFrameworkCore.Migrations;

namespace SatelittiBpms.Data.Migrations
{
    public partial class Adjust_SignerCpfNullnable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_CpfFieldId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.AlterColumn<int>(
                name: "CpfFieldId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "CpfFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_CpfFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "CpfFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_CpfFieldId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.AlterColumn<int>(
                name: "CpfFieldId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "CpfFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_CpfFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "CpfFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
