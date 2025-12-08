using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    LearningPathId = table.Column<int>(type: "int", nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedByManagerId = table.Column<int>(type: "int", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateRequests_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CertificateRequests_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CertificateRequests_LearningPaths_LearningPathId",
                        column: x => x.LearningPathId,
                        principalTable: "LearningPaths",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CertificateRequests_Users_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificateRequests_Users_ReviewedByManagerId",
                        column: x => x.ReviewedByManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequests_CertificateId",
                table: "CertificateRequests",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequests_CourseId",
                table: "CertificateRequests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequests_EmployeeId",
                table: "CertificateRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequests_LearningPathId",
                table: "CertificateRequests",
                column: "LearningPathId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequests_ReviewedByManagerId",
                table: "CertificateRequests",
                column: "ReviewedByManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateRequests");
        }
    }
}
