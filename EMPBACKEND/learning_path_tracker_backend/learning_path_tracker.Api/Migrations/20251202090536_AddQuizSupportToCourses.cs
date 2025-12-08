using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizSupportToCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QuizPassed",
                table: "Enrollments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId1",
                table: "Quizzes",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId1",
                table: "Quizzes",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizPassed",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
