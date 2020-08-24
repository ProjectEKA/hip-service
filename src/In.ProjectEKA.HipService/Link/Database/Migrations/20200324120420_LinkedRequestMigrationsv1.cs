namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class LinkedRequestMigrationsv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("LinkRequest", null, "LinkEnquires");
            migrationBuilder.RenameTable("LinkedCareContext", null, "CareContext");

            migrationBuilder.CreateTable(
                "LinkedAccounts",
                table => new
                {
                    LinkReferenceNumber = table.Column<string>(nullable: false),
                    ConsentManagerUserId = table.Column<string>(nullable: true),
                    PatientReferenceNumber = table.Column<string>(nullable: true),
                    DateTimeStamp = table.Column<string>(nullable: true, defaultValueSql: "now()"),
                    CareContexts = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_LinkedAccounts", x => x.LinkReferenceNumber); });

            migrationBuilder.CreateIndex(
                "IX_CareContext_LinkReferenceNumber",
                "CareContext",
                "LinkReferenceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LinkedAccounts");

            migrationBuilder.CreateIndex(
                "IX_LinkedCareContext_LinkReferenceNumber",
                "LinkedCareContext",
                "LinkReferenceNumber");
        }
    }
}