using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Consent.Database.Migrations
{
    public partial class addConsentManagerIdenitifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConsentManagerId",
                table: "ConsentArtefact",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentManagerId",
                table: "ConsentArtefact");
        }
    }
}
