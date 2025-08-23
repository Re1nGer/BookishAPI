using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterestAreaUser_InterestArea_InterestAreasId",
                table: "InterestAreaUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ReadingPurposeUser_ReadingPurpose_ReadingPurposesId",
                table: "ReadingPurposeUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedBookUser_SelectedBook_SelectedBooksId",
                table: "SelectedBookUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedBook",
                table: "SelectedBook");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReadingPurpose",
                table: "ReadingPurpose");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterestArea",
                table: "InterestArea");

            migrationBuilder.RenameTable(
                name: "SelectedBook",
                newName: "SelectedBooks");

            migrationBuilder.RenameTable(
                name: "ReadingPurpose",
                newName: "ReadingPurposes");

            migrationBuilder.RenameTable(
                name: "InterestArea",
                newName: "InterestAreas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedBooks",
                table: "SelectedBooks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReadingPurposes",
                table: "ReadingPurposes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterestAreas",
                table: "InterestAreas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterestAreaUser_InterestAreas_InterestAreasId",
                table: "InterestAreaUser",
                column: "InterestAreasId",
                principalTable: "InterestAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingPurposeUser_ReadingPurposes_ReadingPurposesId",
                table: "ReadingPurposeUser",
                column: "ReadingPurposesId",
                principalTable: "ReadingPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedBookUser_SelectedBooks_SelectedBooksId",
                table: "SelectedBookUser",
                column: "SelectedBooksId",
                principalTable: "SelectedBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterestAreaUser_InterestAreas_InterestAreasId",
                table: "InterestAreaUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ReadingPurposeUser_ReadingPurposes_ReadingPurposesId",
                table: "ReadingPurposeUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SelectedBookUser_SelectedBooks_SelectedBooksId",
                table: "SelectedBookUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedBooks",
                table: "SelectedBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReadingPurposes",
                table: "ReadingPurposes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterestAreas",
                table: "InterestAreas");

            migrationBuilder.RenameTable(
                name: "SelectedBooks",
                newName: "SelectedBook");

            migrationBuilder.RenameTable(
                name: "ReadingPurposes",
                newName: "ReadingPurpose");

            migrationBuilder.RenameTable(
                name: "InterestAreas",
                newName: "InterestArea");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedBook",
                table: "SelectedBook",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReadingPurpose",
                table: "ReadingPurpose",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterestArea",
                table: "InterestArea",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterestAreaUser_InterestArea_InterestAreasId",
                table: "InterestAreaUser",
                column: "InterestAreasId",
                principalTable: "InterestArea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingPurposeUser_ReadingPurpose_ReadingPurposesId",
                table: "ReadingPurposeUser",
                column: "ReadingPurposesId",
                principalTable: "ReadingPurpose",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedBookUser_SelectedBook_SelectedBooksId",
                table: "SelectedBookUser",
                column: "SelectedBooksId",
                principalTable: "SelectedBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
