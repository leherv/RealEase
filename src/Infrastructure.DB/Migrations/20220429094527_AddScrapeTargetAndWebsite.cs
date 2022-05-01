using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DB.Migrations
{
    public partial class AddScrapeTargetAndWebsite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScrapeTargetId",
                table: "Media",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Website",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Website", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrapeTarget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WebsiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelativeUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrapeTarget_Website_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media_ScrapeTargetId",
                table: "Media",
                column: "ScrapeTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapeTarget_WebsiteId",
                table: "ScrapeTarget",
                column: "WebsiteId");

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

            migrationBuilder.DropTable(
                name: "Website");

            migrationBuilder.DropIndex(
                name: "IX_Media_ScrapeTargetId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ScrapeTargetId",
                table: "Media");
        }
    }
}
