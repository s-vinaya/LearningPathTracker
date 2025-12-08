using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_path_tracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickedCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveredCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpenedCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NotificationTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    IsOpened = table.Column<bool>(type: "bit", nullable: false),
                    IsClicked = table.Column<bool>(type: "bit", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTrackings_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationTrackings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTrackings_NotificationId",
                table: "NotificationTrackings",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTrackings_UserId",
                table: "NotificationTrackings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationTrackings");

            migrationBuilder.DropColumn(
                name: "ClickedCount",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeliveredCount",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "OpenedCount",
                table: "Notifications");
        }
    }
}
