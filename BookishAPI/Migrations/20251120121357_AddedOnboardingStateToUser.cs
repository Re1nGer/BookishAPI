using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedOnboardingStateToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookAmountGoalInYear",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "DailyReminderAt",
                table: "Users",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "IsPremiumUser",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StreakLengthInDays",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeLengthInMinutes",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookAmountGoalInYear",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DailyReminderAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPremiumUser",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StreakLengthInDays",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeLengthInMinutes",
                table: "Users");
        }
    }
}
