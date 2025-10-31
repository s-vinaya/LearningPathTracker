using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPBACKEND.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDeleteConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Enrollments_EnrollmentId1",
                table: "CourseProgresses");

            migrationBuilder.DropIndex(
                name: "IX_CourseProgresses_EnrollmentId1",
                table: "CourseProgresses");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "CourseProgresses");

            migrationBuilder.AlterColumn<decimal>(
                name: "PercentComplete",
                table: "CourseProgresses",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
                table: "CourseProgresses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PercentComplete",
                table: "CourseProgresses",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

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
                name: "FK_CourseProgresses_Enrollments_EnrollmentId1",
                table: "CourseProgresses",
                column: "EnrollmentId1",
                principalTable: "Enrollments",
                principalColumn: "Id");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseProgresses_Courses_CourseId",
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
