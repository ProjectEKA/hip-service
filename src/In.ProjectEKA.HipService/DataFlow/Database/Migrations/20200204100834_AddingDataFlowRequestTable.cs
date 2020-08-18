namespace In.ProjectEKA.HipService.DataFlow.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddingDataFlowRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "DataFlowRequest",
                table => new
                {
                    TransactionId = table.Column<string>(),
                    HealthInformationRequest = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_DataFlowRequest", x => x.TransactionId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "DataFlowRequest");
        }
    }
}