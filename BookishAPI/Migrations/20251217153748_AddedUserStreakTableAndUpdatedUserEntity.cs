using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserStreakTableAndUpdatedUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyReminderEnabled",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "DailyReminderTime",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "NotificationsEnabled",
                table: "UserSettings");

            migrationBuilder.AddColumn<bool>(
                name: "IsNotificationsEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserStreaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    LongestStreak = table.Column<int>(type: "integer", nullable: false),
                    LastActivityDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastActivityUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MinutesReadToday = table.Column<int>(type: "integer", nullable: false),
                    PagesReadToday = table.Column<int>(type: "integer", nullable: false),
                    TodayDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStreaks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStreaks_UserId",
                table: "UserStreaks",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStreaks");

            migrationBuilder.DropColumn(
                name: "IsNotificationsEnabled",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "DailyReminderEnabled",
                table: "UserSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DailyReminderTime",
                table: "UserSettings",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "NotificationsEnabled",
                table: "UserSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
