namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class UniqueTransactionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                "IX_DiscoveryRequest_TransactionId",
                "DiscoveryRequest",
                "TransactionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_DiscoveryRequest_TransactionId",
                "DiscoveryRequest");
        }
    }
}