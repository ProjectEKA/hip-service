using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    public partial class LinkedRequestMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkedAccounts",
                columns: table => new
                {
                    LinkReferenceNumber = table.Column<string>(nullable: false),
                    ConsentManagerUserId = table.Column<string>(nullable: true),
                    PatientReferenceNumber = table.Column<string>(nullable: true),
                    DateTimeStamp = table.Column<string>(nullable: true, defaultValueSql: "now()"),
                    CareContexts = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedAccounts", x => x.LinkReferenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "LinkEnquires",
                columns: table => new
                {
                    LinkReferenceNumber = table.Column<string>(nullable: false),
                    PatientReferenceNumber = table.Column<string>(nullable: true),
                    ConsentManagerId = table.Column<string>(nullable: true),
                    ConsentManagerUserId = table.Column<string>(nullable: true),
                    DateTimeStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkEnquires", x => x.LinkReferenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "CareContext",
                columns: table => new
                {
                    CareContextNumber = table.Column<string>(nullable: false),
                    LinkReferenceNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => new { x.CareContextNumber, x.LinkReferenceNumber });
                    table.ForeignKey(
                        name: "FK_CareContext_LinkEnquires_LinkReferenceNumber",
                        column: x => x.LinkReferenceNumber,
                        principalTable: "LinkEnquires",
                        principalColumn: "LinkReferenceNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareContext_LinkReferenceNumber",
                table: "CareContext",
                column: "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareContext");

            migrationBuilder.DropTable(
                name: "LinkedAccounts");

            migrationBuilder.DropTable(
                name: "LinkEnquires");
        }
    }
}
