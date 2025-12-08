using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId",
                table: "QuizQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuizId1",
                table: "QuizQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizId1",
                table: "QuizAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId1",
                table: "Quizzes",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizId1",
                table: "QuizQuestions",
                column: "QuizId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizId1",
                table: "QuizAttempts",
                column: "QuizId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId",
                table: "QuizAttempts",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId1",
                table: "QuizAttempts",
                column: "QuizId1",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId",
                table: "QuizQuestions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId1",
                table: "QuizQuestions",
                column: "QuizId1",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_QuizAttempts_Quizzes_QuizId",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId1",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId",
                table: "QuizQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId1",
                table: "QuizQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId1",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_QuizId1",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_QuizId1",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizId1",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "QuizId1",
                table: "QuizAttempts");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId",
                table: "QuizAttempts",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Quizzes_QuizId",
                table: "QuizQuestions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
