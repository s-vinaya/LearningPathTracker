using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestDetails",
                table: "Approvals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ManagerId",
                table: "Approvals",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Users_ManagerId",
                table: "Approvals",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Users_ManagerId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_ManagerId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "RequestDetails",
                table: "Approvals");
        }
    }
}
