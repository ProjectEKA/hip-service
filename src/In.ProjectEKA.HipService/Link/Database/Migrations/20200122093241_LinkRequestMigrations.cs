namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class LinkRequestMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkRequest",
                columns: table => new
                {
                    LinkReferenceNumber = table.Column<string>(),
                    PatientReferenceNumber = table.Column<string>(nullable: true),
                    ConsentManagerId = table.Column<string>(nullable: true),
                    ConsentManagerUserId = table.Column<string>(nullable: true),
                    DateTimeStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkRequest", x => x.LinkReferenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "LinkedCareContext",
                columns: table => new
                {
                    CareContextNumber = table.Column<string>(),
                    LinkReferenceNumber = table.Column<string>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("Id", x => new { x.CareContextNumber, x.LinkReferenceNumber });
                    table.ForeignKey(
                        name: "FK_LinkedCareContext_LinkRequest_LinkReferenceNumber",
                        column: x => x.LinkReferenceNumber,
                        principalTable: "LinkRequest",
                        principalColumn: "LinkReferenceNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkedCareContext_LinkReferenceNumber",
                table: "LinkedCareContext",
                column: "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkedCareContext");

            migrationBuilder.DropTable(
                name: "LinkRequest");
        }
    }
}
