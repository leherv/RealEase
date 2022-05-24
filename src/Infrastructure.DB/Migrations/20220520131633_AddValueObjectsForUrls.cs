using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DB.Migrations
{
    public partial class AddValueObjectsForUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Website",
                newName: "Url_Value");

            migrationBuilder.RenameColumn(
                name: "RelativeUrl",
                table: "ScrapeTarget",
                newName: "RelativeUrl_Value");

            migrationBuilder.RenameColumn(
                name: "NewestRelease_Link",
                table: "Media",
                newName: "NewestRelease_ResourceUrl_Value");

            migrationBuilder.AddColumn<DateTime>(
                name: "NewestRelease_Created",
                table: "Media",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewestRelease_Created",
                table: "Media");

            migrationBuilder.RenameColumn(
                name: "Url_Value",
                table: "Website",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "RelativeUrl_Value",
                table: "ScrapeTarget",
                newName: "RelativeUrl");

            migrationBuilder.RenameColumn(
                name: "NewestRelease_ResourceUrl_Value",
                table: "Media",
                newName: "NewestRelease_Link");
        }
    }
}
