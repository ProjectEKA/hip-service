namespace In.ProjectEKA.HipService.Consent.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

    public partial class AddConsentArtefactTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ConsentArtefact",
                table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsentArtefactId = table.Column<string>(nullable: true),
                    ConsentArtefact = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Status = table.Column<string>()
                },
                constraints: table => { table.PrimaryKey("PK_ConsentArtefact", x => x.Id); });

            migrationBuilder.CreateIndex(
                "IX_ConsentArtefact_ConsentArtefactId",
                "ConsentArtefact",
                "ConsentArtefactId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ConsentArtefact");
        }
    }
}