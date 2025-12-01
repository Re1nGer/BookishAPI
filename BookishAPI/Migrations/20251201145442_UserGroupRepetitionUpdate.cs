using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserGroupRepetitionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentIntervalDays",
                table: "UserGroupNotificationSchedules",
                newName: "OffsetIndex");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserGroupNotificationSchedules",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "UserGroupNotificationSchedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "UserGroupNotificationSchedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "UserGroupNotificationSchedules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "UserLocalTime",
                table: "UserGroupNotificationSchedules",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "UserGroupNotificationSchedules");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "UserGroupNotificationSchedules");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "UserGroupNotificationSchedules");

            migrationBuilder.DropColumn(
                name: "UserLocalTime",
                table: "UserGroupNotificationSchedules");

            migrationBuilder.RenameColumn(
                name: "OffsetIndex",
                table: "UserGroupNotificationSchedules",
                newName: "CurrentIntervalDays");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserGroupNotificationSchedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
