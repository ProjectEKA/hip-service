using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.OtpService.Migrations
{
    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtpRequests",
                columns: table => new
                {
                    SessionId = table.Column<string>(nullable: false),
                    DateTimeStamp = table.Column<string>(nullable: false),
                    OtpToken = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpRequests", x => x.SessionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpRequests");
        }
    }
}
