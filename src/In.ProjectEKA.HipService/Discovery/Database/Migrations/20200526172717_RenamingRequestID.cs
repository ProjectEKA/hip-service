namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RenamingRequestID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("RequestId", "DiscoveryRequest", "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_DiscoveryRequest_TransactionId",
                "DiscoveryRequest");

            migrationBuilder.DropColumn(
                "TransactionId",
                "DiscoveryRequest");

            migrationBuilder.AddColumn<string>(
                "RequestId",
                "DiscoveryRequest",
                "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                "IX_DiscoveryRequest_RequestId",
                "DiscoveryRequest",
                "RequestId",
                unique: true);
        }
    }
}