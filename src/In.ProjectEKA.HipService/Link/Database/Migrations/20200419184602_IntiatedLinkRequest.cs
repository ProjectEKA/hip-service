namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class IntiatedLinkRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "InitiatedLinkRequest",
                table => new
                {
                    RequestId = table.Column<string>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true),
                    LinkReferenceNumber = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false, defaultValueSql: "false"),
                    DateTimeStamp = table.Column<string>(nullable: true, defaultValueSql: "now()")
                },
                constraints: table => { table.PrimaryKey("PK_InitiatedLinkRequest", x => x.RequestId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "InitiatedLinkRequest");
        }
    }
}