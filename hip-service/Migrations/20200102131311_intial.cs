using Microsoft.EntityFrameworkCore.Migrations;

namespace hip_service.Migrations
{
    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkRequest",
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
                    table.PrimaryKey("PK_LinkRequest", x => x.LinkReferenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "LinkedCareContext",
                columns: table => new
                {
                    CareContextNumber = table.Column<string>(nullable: false),
                    LinkReferenceNumber = table.Column<string>(nullable: true),
                    LinkRequestLinkReferenceNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedCareContext", x => x.CareContextNumber);
                    table.ForeignKey(
                        name: "FK_LinkedCareContext_LinkRequest_LinkRequestLinkReferenceNumber",
                        column: x => x.LinkRequestLinkReferenceNumber,
                        principalTable: "LinkRequest",
                        principalColumn: "LinkReferenceNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkedCareContext_LinkRequestLinkReferenceNumber",
                table: "LinkedCareContext",
                column: "LinkRequestLinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkedCareContext");

            migrationBuilder.DropTable(
                name: "LinkRequest");
        }
    }
}
