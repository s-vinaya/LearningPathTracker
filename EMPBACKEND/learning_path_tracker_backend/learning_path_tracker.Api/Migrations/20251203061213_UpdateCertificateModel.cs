using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertificateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId1",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LearningPaths_LearningPathId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId1",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Quizzes");

            migrationBuilder.RenameColumn(
                name: "CertificateNumber",
                table: "Certificates",
                newName: "EmployeeName");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Certificates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "AverageScore",
                table: "Certificates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CertificateId",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificateType",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CourseName",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LearningPathId",
                table: "Certificates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningPathName",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes",
                column: "CourseId");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Certificates_CourseId' AND object_id = OBJECT_ID('Certificates'))
                BEGIN
                    CREATE INDEX [IX_Certificates_CourseId] ON [Certificates] ([CourseId]);
                END
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_LearningPathId",
                table: "Certificates",
                column: "LearningPathId");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Certificates_Courses_CourseId')
                BEGIN
                    ALTER TABLE [Certificates] ADD CONSTRAINT [FK_Certificates_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]);
                END
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_LearningPaths_LearningPathId",
                table: "Certificates",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LearningPaths_LearningPathId",
                table: "Quizzes",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Certificates_Courses_CourseId')
                BEGIN
                    ALTER TABLE [Certificates] DROP CONSTRAINT [FK_Certificates_Courses_CourseId];
                END
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_LearningPaths_LearningPathId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_LearningPaths_LearningPathId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_CourseId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_LearningPathId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "CertificateType",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "CourseName",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearningPathId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearningPathName",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "EmployeeName",
                table: "Certificates",
                newName: "CertificateNumber");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_LearningPaths_LearningPathId",
                table: "Quizzes",
                column: "LearningPathId",
                principalTable: "LearningPaths",
                principalColumn: "Id");
        }
    }
}
