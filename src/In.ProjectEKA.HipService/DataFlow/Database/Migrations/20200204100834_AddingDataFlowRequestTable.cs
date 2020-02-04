using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.DataFlow.Database.Migrations
{
    public partial class AddingDataFlowRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataFlowRequest",
                columns: table => new
                {
                    TransactionId = table.Column<string>(nullable: false),
                    HealthInformationRequest = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataFlowRequest", x => x.TransactionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataFlowRequest");
        }
    }
}
