namespace In.ProjectEKA.HipService.Discovery.Database.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

    public partial class AddDiscoveryRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "DiscoveryRequest",
                table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionId = table.Column<string>(maxLength: 50),
                    ConsentManagerUserId = table.Column<string>(maxLength: 50),
                    Timestamp = table.Column<DateTime>(nullable: false, defaultValueSql: "now()")
                },
                constraints: table => { table.PrimaryKey("PK_DiscoveryRequest", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "DiscoveryRequest");
        }
    }
}