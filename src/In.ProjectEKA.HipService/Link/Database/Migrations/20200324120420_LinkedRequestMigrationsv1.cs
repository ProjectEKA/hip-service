using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    public partial class LinkedRequestMigrationsv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("LinkRequest", null, "LinkEnquires");
            migrationBuilder.RenameTable("LinkedCareContext", null, "CareContext");

            migrationBuilder.CreateTable(
                name: "LinkedAccounts",
                columns: table => new
                {
                    LinkReferenceNumber = table.Column<string>(nullable: false),
                    ConsentManagerUserId = table.Column<string>(nullable: true),
                    PatientReferenceNumber = table.Column<string>(nullable: true),
                    DateTimeStamp = table.Column<string>(nullable: true, defaultValueSql: "now()"),
                    CareContexts = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedAccounts", x => x.LinkReferenceNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareContext_LinkReferenceNumber",
                table: "CareContext",
                column: "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkedAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedCareContext_LinkReferenceNumber",
                table: "LinkedCareContext",
                column: "LinkReferenceNumber");
        }
    }
}
