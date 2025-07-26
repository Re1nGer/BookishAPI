using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedIconIdToSpaceGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IconId",
                table: "SpacedRepetitionGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconId",
                table: "SpacedRepetitionGroups");
        }
    }
}
