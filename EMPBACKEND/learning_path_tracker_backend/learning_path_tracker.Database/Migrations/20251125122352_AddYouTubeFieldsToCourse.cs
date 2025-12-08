using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddYouTubeFieldsToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoDuration",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VideoDuration",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Courses");
        }
    }
}
