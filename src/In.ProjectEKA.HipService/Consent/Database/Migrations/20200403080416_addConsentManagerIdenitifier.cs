namespace In.ProjectEKA.HipService.Consent.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class addConsentManagerIdenitifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "ConsentManagerId",
                "ConsentArtefact",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "ConsentManagerId",
                "ConsentArtefact");
        }
    }
}