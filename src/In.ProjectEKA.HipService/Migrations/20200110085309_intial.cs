namespace hip_service.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "LinkRequest",
                table => new
                {
                    LinkReferenceNumber = table.Column<string>(),
                    PatientReferenceNumber = table.Column<string>(nullable: true),
                    ConsentManagerId = table.Column<string>(nullable: true),
                    ConsentManagerUserId = table.Column<string>(nullable: true),
                    DateTimeStamp = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_LinkRequest", x => x.LinkReferenceNumber); });

            migrationBuilder.CreateTable(
                "OtpRequests",
                table => new
                {
                    SessionId = table.Column<string>(),
                    DateTimeStamp = table.Column<string>(nullable: true),
                    OtpToken = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_OtpRequests", x => x.SessionId); });

            migrationBuilder.CreateTable(
                "LinkedCareContexts",
                table => new
                {
                    CareContextNumber = table.Column<string>(),
                    LinkReferenceNumber = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => new {x.CareContextNumber, x.LinkReferenceNumber});
                    table.ForeignKey(
                        "FK_LinkedCareContexts_LinkRequest_LinkReferenceNumber",
                        x => x.LinkReferenceNumber,
                        "LinkRequest",
                        "LinkReferenceNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_LinkedCareContexts_LinkReferenceNumber",
                "LinkedCareContexts",
                "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LinkedCareContexts");

            migrationBuilder.DropTable(
                "OtpRequests");

            migrationBuilder.DropTable(
                "LinkRequest");
        }
    }
}