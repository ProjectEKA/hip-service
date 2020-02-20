namespace In.ProjectEKA.HipService.DataFlow.Database.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddLinkDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "LinkData",
                table => new
                {
                    LinkId = table.Column<string>(),
                    Data = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_LinkData", x => x.LinkId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LinkData");
        }
    }
}