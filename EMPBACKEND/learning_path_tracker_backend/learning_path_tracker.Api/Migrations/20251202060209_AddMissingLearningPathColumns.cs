using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingLearningPathColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add CreatedBy column
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LearningPaths",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "System");

            // Rename CreatedAt to CreatedOn
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "LearningPaths",
                newName: "CreatedOn");

            // Rename EstimatedDuration to EstimatedHours
            migrationBuilder.RenameColumn(
                name: "EstimatedDuration",
                table: "LearningPaths",
                newName: "EstimatedHours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the changes
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LearningPaths");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "LearningPaths",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "EstimatedHours",
                table: "LearningPaths",
                newName: "EstimatedDuration");
        }
    }
}
