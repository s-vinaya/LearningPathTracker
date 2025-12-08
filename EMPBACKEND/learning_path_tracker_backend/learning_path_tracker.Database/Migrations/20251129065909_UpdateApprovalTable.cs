using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApprovalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Users_ManagerId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_ManagerId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "RelatedEntityId",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "ReviewedDate",
                table: "Approvals",
                newName: "ReviewedAt");

            migrationBuilder.RenameColumn(
                name: "RequestedDate",
                table: "Approvals",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "RequestType",
                table: "Approvals",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Comments",
                table: "Approvals",
                newName: "ReviewerComments");

            migrationBuilder.AddColumn<string>(
                name: "Payload",
                table: "Approvals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payload",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Approvals",
                newName: "RequestType");

            migrationBuilder.RenameColumn(
                name: "ReviewerComments",
                table: "Approvals",
                newName: "Comments");

            migrationBuilder.RenameColumn(
                name: "ReviewedAt",
                table: "Approvals",
                newName: "ReviewedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Approvals",
                newName: "RequestedDate");

            migrationBuilder.AddColumn<int>(
                name: "RelatedEntityId",
                table: "Approvals",
                type: "int",
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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
