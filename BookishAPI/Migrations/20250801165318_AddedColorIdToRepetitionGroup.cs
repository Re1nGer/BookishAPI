using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedColorIdToRepetitionGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ColorId",
                table: "SpacedRepetitionGroups",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "SpacedRepetitionGroups");
        }
    }
}
