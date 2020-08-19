namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RenamingTranscationID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("TransactionId", "DiscoveryRequest", "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_DiscoveryRequest_RequestId",
                "DiscoveryRequest");

            migrationBuilder.DropColumn(
                "RequestId",
                "DiscoveryRequest");

            migrationBuilder.AddColumn<string>(
                "TransactionId",
                "DiscoveryRequest",
                "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                "IX_DiscoveryRequest_TransactionId",
                "DiscoveryRequest",
                "TransactionId",
                unique: true);
        }
    }
}