using Microsoft.EntityFrameworkCore.Migrations;

namespace hip_service.Migrations
{
    public partial class update_002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtpRequests",
                columns: table => new
                {
                    LinkReferenceNumber = table.Column<string>(nullable: false),
                    DateTimeStamp = table.Column<string>(nullable: true),
                    OtpToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpRequests", x => x.LinkReferenceNumber);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpRequests");
        }
    }
}
