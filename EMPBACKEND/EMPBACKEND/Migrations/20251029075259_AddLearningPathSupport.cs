using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPBACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningPathSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningPlans_Courses_CourseId",
                table: "LearningPlans");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LearningPlans",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LearningPathId",
                table: "LearningPlans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LearningPaths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPaths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningPathCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearningPathId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPathCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningPathCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningPathCourses_LearningPaths_LearningPathId",
                        column: x => x.LearningPathId,
                        principalTable: "LearningPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningPlans_LearningPathId",
                table: "LearningPlans",
                column: "LearningPathId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourses_CourseId",
                table: "LearningPathCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourses_LearningPathId",
                table: "LearningPathCourses",
                column: "LearningPathId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningPlans_Courses_CourseId",
                table: "LearningPlans",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningPlans_LearningPaths_LearningPathId",
                table: "LearningPlans",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningPlans_Courses_CourseId",
                table: "LearningPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningPlans_LearningPaths_LearningPathId",
                table: "LearningPlans");

            migrationBuilder.DropTable(
                name: "LearningPathCourses");

            migrationBuilder.DropTable(
                name: "LearningPaths");

            migrationBuilder.DropIndex(
                name: "IX_LearningPlans_LearningPathId",
                table: "LearningPlans");

            migrationBuilder.DropColumn(
                name: "LearningPathId",
                table: "LearningPlans");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LearningPlans",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningPlans_Courses_CourseId",
                table: "LearningPlans",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
