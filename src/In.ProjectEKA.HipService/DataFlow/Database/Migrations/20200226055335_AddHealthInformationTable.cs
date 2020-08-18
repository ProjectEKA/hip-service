namespace In.ProjectEKA.HipService.DataFlow.Database.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddHealthInformationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("HealthInformation", table => new
            {
                InformationId = table.Column<string>(nullable: false),
                Data = table.Column<string>(nullable: true),
                DateCreated = table.Column<DateTime>(nullable: false),
                Token = table.Column<string>(nullable: true)
            },
                constraints: table => { table.PrimaryKey("PK_HealthInformation", x => x.InformationId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("HealthInformation");
        }
    }
}