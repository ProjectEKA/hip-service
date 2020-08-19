namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class PatientReferenceNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "PatientReferenceNumber",
                "DiscoveryRequest",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "PatientReferenceNumber",
                "DiscoveryRequest");
        }
    }
}