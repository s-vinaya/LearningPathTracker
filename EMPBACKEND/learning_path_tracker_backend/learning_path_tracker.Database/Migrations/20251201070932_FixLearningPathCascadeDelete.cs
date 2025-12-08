using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixLearningPathCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LearningPathId",
                table: "Approvals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_LearningPathId",
                table: "Approvals",
                column: "LearningPathId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_LearningPaths_LearningPathId",
                table: "Approvals",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_LearningPaths_LearningPathId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_LearningPathId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "LearningPathId",
                table: "Approvals");
        }
    }
}
