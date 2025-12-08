using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelToLearningPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "LearningPaths",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "LearningPaths");
        }
    }
}
