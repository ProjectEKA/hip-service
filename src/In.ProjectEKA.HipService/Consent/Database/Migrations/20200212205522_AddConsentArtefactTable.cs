using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace In.ProjectEKA.HipService.Consent.Database.Migrations
{
    public partial class AddConsentArtefactTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsentArtefact",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsentArtefactId = table.Column<string>(nullable: true),
                    ConsentArtefact = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentArtefact", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsentArtefact_ConsentArtefactId",
                table: "ConsentArtefact",
                column: "ConsentArtefactId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsentArtefact");
        }
    }
}
