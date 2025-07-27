using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedScheduler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemindAt",
                table: "SpacedRepetitionGroups",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SpacedRepetitionGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FirebaseResponse = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupNotificationSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    CurrentIntervalDays = table.Column<int>(type: "integer", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupNotificationSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroupNotificationSchedules_SpacedRepetitionGroups_Group~",
                        column: x => x.GroupId,
                        principalTable: "SpacedRepetitionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPushTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPushTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_UserId_GroupId_SentAt",
                table: "NotificationLogs",
                columns: new[] { "UserId", "GroupId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupNotificationSchedules_GroupId",
                table: "UserGroupNotificationSchedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupNotificationSchedules_ScheduledTime",
                table: "UserGroupNotificationSchedules",
                column: "ScheduledTime");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupNotificationSchedules_UserId_GroupId",
                table: "UserGroupNotificationSchedules",
                columns: new[] { "UserId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPushTokens_DeviceToken",
                table: "UserPushTokens",
                column: "DeviceToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserPushTokens_UserId_Platform",
                table: "UserPushTokens",
                columns: new[] { "UserId", "Platform" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropTable(
                name: "UserGroupNotificationSchedules");

            migrationBuilder.DropTable(
                name: "UserPushTokens");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SpacedRepetitionGroups");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "SpacedRepetitionGroups",
                newName: "RemindAt");
        }
    }
}
