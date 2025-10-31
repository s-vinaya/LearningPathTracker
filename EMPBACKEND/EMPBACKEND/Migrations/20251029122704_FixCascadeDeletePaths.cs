using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPBACKEND.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeletePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses");

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId1",
                table: "CourseProgresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgresses_EnrollmentId1",
                table: "CourseProgresses",
                column: "EnrollmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_Enrollments_EnrollmentId1",
                table: "CourseProgresses",
                column: "EnrollmentId1",
                principalTable: "Enrollments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Enrollments_EnrollmentId1",
                table: "CourseProgresses");

            migrationBuilder.DropIndex(
                name: "IX_CourseProgresses_EnrollmentId1",
                table: "CourseProgresses");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "CourseProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
