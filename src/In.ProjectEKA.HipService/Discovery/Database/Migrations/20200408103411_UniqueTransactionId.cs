using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    public partial class UniqueTransactionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DiscoveryRequest_TransactionId",
                table: "DiscoveryRequest",
                column: "TransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiscoveryRequest_TransactionId",
                table: "DiscoveryRequest");
        }
    }
}
