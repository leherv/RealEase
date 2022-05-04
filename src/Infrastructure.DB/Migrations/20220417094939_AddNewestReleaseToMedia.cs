using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DB.Migrations
{
    public partial class AddNewestReleaseToMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewestRelease_Link",
                table: "Media",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewestRelease_ReleaseNumber_Major",
                table: "Media",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewestRelease_ReleaseNumber_Minor",
                table: "Media",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewestRelease_Link",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "NewestRelease_ReleaseNumber_Major",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "NewestRelease_ReleaseNumber_Minor",
                table: "Media");
        }
    }
}
