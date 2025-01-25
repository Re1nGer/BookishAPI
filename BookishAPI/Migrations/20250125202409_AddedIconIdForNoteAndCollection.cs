using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedIconIdForNoteAndCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IconId",
                table: "QuoteCollections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IconId",
                table: "NoteCollections",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconId",
                table: "QuoteCollections");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "NoteCollections");
        }
    }
}
