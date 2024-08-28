using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class Adjust_SetSignerFieldsNullnable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivities_Fields_ExpirationDateFieldId",
                table: "SignerIntegrationActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_NameFieldId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_EmailFieldId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_NameFieldId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.AlterColumn<int>(
                name: "NameFieldId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EmailFieldId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NameFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExpirationDateFieldId",
                table: "SignerIntegrationActivities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivities_Fields_ExpirationDateFieldId",
                table: "SignerIntegrationActivities",
                column: "ExpirationDateFieldId",
                principalTable: "Fields",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "EmailFieldId",
                principalTable: "Fields",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_NameFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "NameFieldId",
                principalTable: "Fields",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_EmailFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "EmailFieldId",
                principalTable: "Fields",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_NameFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "NameFieldId",
                principalTable: "Fields",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivities_Fields_ExpirationDateFieldId",
                table: "SignerIntegrationActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_NameFieldId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_EmailFieldId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_NameFieldId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.AlterColumn<int>(
                name: "NameFieldId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmailFieldId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NameFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExpirationDateFieldId",
                table: "SignerIntegrationActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivities_Fields_ExpirationDateFieldId",
                table: "SignerIntegrationActivities",
                column: "ExpirationDateFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "EmailFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Fields_NameFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "NameFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_EmailFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "EmailFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Fields_NameFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "NameFieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
