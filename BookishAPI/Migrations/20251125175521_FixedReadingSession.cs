using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixedReadingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ReadingSessions");

            migrationBuilder.DropColumn(
                name: "StartPage",
                table: "ReadingSessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ReadingSessions");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ReadingSessions",
                newName: "DurationInSeconds");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "ReadingSessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "ReadingSessions");

            migrationBuilder.RenameColumn(
                name: "DurationInSeconds",
                table: "ReadingSessions",
                newName: "Status");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "ReadingSessions",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "StartPage",
                table: "ReadingSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ReadingSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
