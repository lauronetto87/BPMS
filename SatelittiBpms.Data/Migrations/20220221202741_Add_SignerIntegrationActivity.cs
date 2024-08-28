using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SatelittiBpms.Data.Migrations
{
    public partial class Add_SignerIntegrationActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SignerIntegrationActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EnvelopeTitle = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Language = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Segment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SendReminders = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SignatoryAccessAuthentication = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AuthorizeEnablePriorAuthorizationOfTheDocument = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AuthorizeAccessAuthentication = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    ExpirationDateFieldId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignerIntegrationActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivities_Fields_ExpirationDateFieldId",
                        column: x => x.ExpirationDateFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignerIntegrationActivityAuthorizers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RegistrationLocation = table.Column<int>(type: "int", nullable: false),
                    NameFieldId = table.Column<int>(type: "int", nullable: false),
                    CpfFieldId = table.Column<int>(type: "int", nullable: false),
                    EmailFieldId = table.Column<int>(type: "int", nullable: false),
                    SignerIntegrationActivityId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignerIntegrationActivityAuthorizers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivityAuthorizers_Fields_CpfFieldId",
                        column: x => x.CpfFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivityAuthorizers_Fields_EmailFieldId",
                        column: x => x.EmailFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivityAuthorizers_Fields_NameFieldId",
                        column: x => x.NameFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivityAuthorizers_SignerIntegrationActivi~",
                        column: x => x.SignerIntegrationActivityId,
                        principalTable: "SignerIntegrationActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignerIntegrationActivityFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileFieldId = table.Column<int>(type: "int", nullable: false),
                    SignerIntegrationActivityId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignerIntegrationActivityFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivityFiles_Fields_FileFieldId",
                        column: x => x.FileFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivityFiles_SignerIntegrationActivities_S~",
                        column: x => x.SignerIntegrationActivityId,
                        principalTable: "SignerIntegrationActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignerIntegrationActivitySignatories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RegistrationLocation = table.Column<int>(type: "int", nullable: false),
                    SubscriberTypeId = table.Column<int>(type: "int", nullable: false),
                    SignatureTypeId = table.Column<int>(type: "int", nullable: false),
                    NameFieldId = table.Column<int>(type: "int", nullable: false),
                    CpfFieldId = table.Column<int>(type: "int", nullable: false),
                    EmailFieldId = table.Column<int>(type: "int", nullable: false),
                    SignerIntegrationActivityId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignerIntegrationActivitySignatories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivitySignatories_Fields_CpfFieldId",
                        column: x => x.CpfFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivitySignatories_Fields_EmailFieldId",
                        column: x => x.EmailFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivitySignatories_Fields_NameFieldId",
                        column: x => x.NameFieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignerIntegrationActivitySignatories_SignerIntegrationActivi~",
                        column: x => x.SignerIntegrationActivityId,
                        principalTable: "SignerIntegrationActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivities_ActivityId",
                table: "SignerIntegrationActivities",
                column: "ActivityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivities_ExpirationDateFieldId",
                table: "SignerIntegrationActivities",
                column: "ExpirationDateFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityAuthorizers_CpfFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "CpfFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityAuthorizers_EmailFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "EmailFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityAuthorizers_NameFieldId",
                table: "SignerIntegrationActivityAuthorizers",
                column: "NameFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityAuthorizers_SignerIntegrationActivi~",
                table: "SignerIntegrationActivityAuthorizers",
                column: "SignerIntegrationActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityFiles_FileFieldId",
                table: "SignerIntegrationActivityFiles",
                column: "FileFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivityFiles_SignerIntegrationActivityId",
                table: "SignerIntegrationActivityFiles",
                column: "SignerIntegrationActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivitySignatories_CpfFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "CpfFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivitySignatories_EmailFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "EmailFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivitySignatories_NameFieldId",
                table: "SignerIntegrationActivitySignatories",
                column: "NameFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SignerIntegrationActivitySignatories_SignerIntegrationActivi~",
                table: "SignerIntegrationActivitySignatories",
                column: "SignerIntegrationActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignerIntegrationActivityAuthorizers");

            migrationBuilder.DropTable(
                name: "SignerIntegrationActivityFiles");

            migrationBuilder.DropTable(
                name: "SignerIntegrationActivitySignatories");

            migrationBuilder.DropTable(
                name: "SignerIntegrationActivities");
        }
    }
}
