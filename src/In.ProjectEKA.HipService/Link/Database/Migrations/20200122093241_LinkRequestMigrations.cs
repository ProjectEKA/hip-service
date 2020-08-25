namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class LinkRequestMigrations : Migration
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
                "LinkedCareContext",
                table => new
                {
                    CareContextNumber = table.Column<string>(),
                    LinkReferenceNumber = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => new {x.CareContextNumber, x.LinkReferenceNumber});
                    table.ForeignKey(
                        "FK_LinkedCareContext_LinkRequest_LinkReferenceNumber",
                        x => x.LinkReferenceNumber,
                        "LinkRequest",
                        "LinkReferenceNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_LinkedCareContext_LinkReferenceNumber",
                "LinkedCareContext",
                "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LinkedCareContext");

            migrationBuilder.DropTable(
                "LinkRequest");
        }
    }
}