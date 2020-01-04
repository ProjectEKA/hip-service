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
                name: "OtpRequests",
                columns: table => new
                {
                    SessionId = table.Column<string>(nullable: false),
                    DateTimeStamp = table.Column<string>(nullable: true),
                    OtpToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpRequests", x => x.SessionId);
                });

            migrationBuilder.CreateTable(
                name: "LinkedCareContexts",
                columns: table => new
                {
                    CareContextNumber = table.Column<string>(nullable: false),
                    LinkReferenceNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => new { x.CareContextNumber, x.LinkReferenceNumber });
                    table.ForeignKey(
                        name: "FK_LinkedCareContexts_LinkRequest_LinkReferenceNumber",
                        column: x => x.LinkReferenceNumber,
                        principalTable: "LinkRequest",
                        principalColumn: "LinkReferenceNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkedCareContexts_LinkReferenceNumber",
                table: "LinkedCareContexts",
                column: "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkedCareContexts");

            migrationBuilder.DropTable(
                name: "OtpRequests");

            migrationBuilder.DropTable(
                name: "LinkRequest");
        }
    }
}
