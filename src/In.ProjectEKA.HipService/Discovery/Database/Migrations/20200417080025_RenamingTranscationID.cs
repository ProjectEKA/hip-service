using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    public partial class RenamingTranscationID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("TransactionId", "DiscoveryRequest", "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiscoveryRequest_RequestId",
                table: "DiscoveryRequest");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "DiscoveryRequest");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "DiscoveryRequest",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveryRequest_TransactionId",
                table: "DiscoveryRequest",
                column: "TransactionId",
                unique: true);
        }
    }
}
