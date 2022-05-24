using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DB.Migrations
{
    public partial class MultipleScrapeTargetsPerMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_ScrapeTarget_ScrapeTargetId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_ScrapeTargetId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ScrapeTargetId",
                table: "Media");

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "ScrapeTarget",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScrapeTarget_MediaId",
                table: "ScrapeTarget",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScrapeTarget_Media_MediaId",
                table: "ScrapeTarget",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScrapeTarget_Media_MediaId",
                table: "ScrapeTarget");

            migrationBuilder.DropIndex(
                name: "IX_ScrapeTarget_MediaId",
                table: "ScrapeTarget");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "ScrapeTarget");

            migrationBuilder.AddColumn<Guid>(
                name: "ScrapeTargetId",
                table: "Media",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_ScrapeTargetId",
                table: "Media",
                column: "ScrapeTargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_ScrapeTarget_ScrapeTargetId",
                table: "Media",
                column: "ScrapeTargetId",
                principalTable: "ScrapeTarget",
                principalColumn: "Id");
        }
    }
}
