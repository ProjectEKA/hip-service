using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    public partial class RenamingRequestID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("RequestId", "DiscoveryRequest", "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiscoveryRequest_TransactionId",
                table: "DiscoveryRequest");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "DiscoveryRequest");

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "DiscoveryRequest",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveryRequest_RequestId",
                table: "DiscoveryRequest",
                column: "RequestId",
                unique: true);
        }
    }
}
