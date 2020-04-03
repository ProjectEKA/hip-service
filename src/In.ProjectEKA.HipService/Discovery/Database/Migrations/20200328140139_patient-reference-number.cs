using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    public partial class PatientReferenceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientReferenceNumber",
                table: "DiscoveryRequest",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientReferenceNumber",
                table: "DiscoveryRequest");
        }
    }
}