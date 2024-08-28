using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelittiBpms.Data.Migrations
{
    public partial class Add_OriginActivityId_SignerIntegrationActivityAuthorizer_And_SignerIntegrationActivitySignatory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginActivityId",
                table: "SignerIntegrationActivitySignatories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginActivityId",
                table: "SignerIntegrationActivityAuthorizers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivitySignatories_OriginActivityId",
                table: "SignerIntegrationActivitySignatories",
                column: "OriginActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityAuthorizers_OriginActivityId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "OriginActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Activities_OriginActivi~",
                table: "SignerIntegrationActivityAuthorizers",
                column: "OriginActivityId",
                principalTable: "Activities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Activities_OriginActivi~",
                table: "SignerIntegrationActivitySignatories",
                column: "OriginActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivityAuthorizers_Activities_OriginActivi~",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropForeignKey(
                name: "FK_SignerIntegrationActivitySignatories_Activities_OriginActivi~",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.DropIndex(
                name: "IX_SignerIntegrationActivitySignatories_OriginActivityId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.DropIndex(
                name: "IX_SignerIntegrationActivityAuthorizers_OriginActivityId",
                table: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropColumn(
                name: "OriginActivityId",
                table: "SignerIntegrationActivitySignatories");

            migrationBuilder.DropColumn(
                name: "OriginActivityId",
                table: "SignerIntegrationActivityAuthorizers");
        }
    }
}
