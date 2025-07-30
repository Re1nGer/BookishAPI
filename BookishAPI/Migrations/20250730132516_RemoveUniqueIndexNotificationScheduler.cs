using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndexNotificationScheduler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserGroupNotificationSchedules_UserId_GroupId",
                table: "UserGroupNotificationSchedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserGroupNotificationSchedules_UserId_GroupId",
                table: "UserGroupNotificationSchedules",
                columns: new[] { "UserId", "GroupId" },
                unique: true);
        }
    }
}
