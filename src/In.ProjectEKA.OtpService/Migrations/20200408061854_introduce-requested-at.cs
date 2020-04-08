using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace In.ProjectEKA.OtpService.Migrations
{
    // ReSharper disable once UnusedType.Global
    public partial class IntroduceRequestedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTimeStamp",
                table: "OtpRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "OtpRequests",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "OtpRequests");

            migrationBuilder.AddColumn<string>(
                name: "DateTimeStamp",
                table: "OtpRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}