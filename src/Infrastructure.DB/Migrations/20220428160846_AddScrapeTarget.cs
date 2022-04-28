using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DB.Migrations
{
    public partial class AddScrapeTarget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScrapeTargetId",
                table: "Media",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScrapeTarget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeTarget", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_ScrapeTarget_ScrapeTargetId",
                table: "Media");

            migrationBuilder.DropTable(
                name: "ScrapeTarget");

            migrationBuilder.DropIndex(
                name: "IX_Media_ScrapeTargetId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ScrapeTargetId",
                table: "Media");
        }
    }
}
