using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedConfiguredCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionNotes_NoteCollection_NoteCollectionsId",
                table: "CollectionNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectionNotes_Notes_NotesId",
                table: "CollectionNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectionQuotes_QuoteCollection_QuoteCollectionsId",
                table: "CollectionQuotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectionQuotes_Quotes_QuotesId",
                table: "CollectionQuotes");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteCollection_Users_UserId",
                table: "NoteCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_NotesSpacedRepetition_Notes_NotesId",
                table: "NotesSpacedRepetition");

            migrationBuilder.DropForeignKey(
                name: "FK_NotesSpacedRepetition_SpacedRepetitionGroups_SpacedRepetiti~",
                table: "NotesSpacedRepetition");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteCollection_Users_UserId",
                table: "QuoteCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotesSpacedRepetition_Quotes_QuotesId",
                table: "QuotesSpacedRepetition");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotesSpacedRepetition_SpacedRepetitionGroups_SpacedRepetit~",
                table: "QuotesSpacedRepetition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuotesSpacedRepetition",
                table: "QuotesSpacedRepetition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuoteCollection",
                table: "QuoteCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotesSpacedRepetition",
                table: "NotesSpacedRepetition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteCollection",
                table: "NoteCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CollectionQuotes",
                table: "CollectionQuotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CollectionNotes",
                table: "CollectionNotes");

            migrationBuilder.RenameTable(
                name: "QuotesSpacedRepetition",
                newName: "QuoteSpacedRepetitionGroup");

            migrationBuilder.RenameTable(
                name: "QuoteCollection",
                newName: "QuoteCollections");

            migrationBuilder.RenameTable(
                name: "NotesSpacedRepetition",
                newName: "NoteSpacedRepetitionGroup");

            migrationBuilder.RenameTable(
                name: "NoteCollection",
                newName: "NoteCollections");

            migrationBuilder.RenameTable(
                name: "CollectionQuotes",
                newName: "QuoteQuoteCollection");

            migrationBuilder.RenameTable(
                name: "CollectionNotes",
                newName: "NoteNoteCollection");

            migrationBuilder.RenameIndex(
                name: "IX_QuotesSpacedRepetition_SpacedRepetitionGroupsId",
                table: "QuoteSpacedRepetitionGroup",
                newName: "IX_QuoteSpacedRepetitionGroup_SpacedRepetitionGroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_QuoteCollection_UserId",
                table: "QuoteCollections",
                newName: "IX_QuoteCollections_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotesSpacedRepetition_SpacedRepetitionGroupsId",
                table: "NoteSpacedRepetitionGroup",
                newName: "IX_NoteSpacedRepetitionGroup_SpacedRepetitionGroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteCollection_UserId",
                table: "NoteCollections",
                newName: "IX_NoteCollections_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionQuotes_QuotesId",
                table: "QuoteQuoteCollection",
                newName: "IX_QuoteQuoteCollection_QuotesId");

            migrationBuilder.RenameIndex(
                name: "IX_CollectionNotes_NotesId",
                table: "NoteNoteCollection",
                newName: "IX_NoteNoteCollection_NotesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuoteSpacedRepetitionGroup",
                table: "QuoteSpacedRepetitionGroup",
                columns: new[] { "QuotesId", "SpacedRepetitionGroupsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuoteCollections",
                table: "QuoteCollections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteSpacedRepetitionGroup",
                table: "NoteSpacedRepetitionGroup",
                columns: new[] { "NotesId", "SpacedRepetitionGroupsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteCollections",
                table: "NoteCollections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuoteQuoteCollection",
                table: "QuoteQuoteCollection",
                columns: new[] { "QuoteCollectionsId", "QuotesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteNoteCollection",
                table: "NoteNoteCollection",
                columns: new[] { "NoteCollectionsId", "NotesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_NoteCollections_Users_UserId",
                table: "NoteCollections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteNoteCollection_NoteCollections_NoteCollectionsId",
                table: "NoteNoteCollection",
                column: "NoteCollectionsId",
                principalTable: "NoteCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteNoteCollection_Notes_NotesId",
                table: "NoteNoteCollection",
                column: "NotesId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteSpacedRepetitionGroup_Notes_NotesId",
                table: "NoteSpacedRepetitionGroup",
                column: "NotesId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteSpacedRepetitionGroup_SpacedRepetitionGroups_SpacedRepe~",
                table: "NoteSpacedRepetitionGroup",
                column: "SpacedRepetitionGroupsId",
                principalTable: "SpacedRepetitionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteCollections_Users_UserId",
                table: "QuoteCollections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteQuoteCollection_QuoteCollections_QuoteCollectionsId",
                table: "QuoteQuoteCollection",
                column: "QuoteCollectionsId",
                principalTable: "QuoteCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteQuoteCollection_Quotes_QuotesId",
                table: "QuoteQuoteCollection",
                column: "QuotesId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteSpacedRepetitionGroup_Quotes_QuotesId",
                table: "QuoteSpacedRepetitionGroup",
                column: "QuotesId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteSpacedRepetitionGroup_SpacedRepetitionGroups_SpacedRep~",
                table: "QuoteSpacedRepetitionGroup",
                column: "SpacedRepetitionGroupsId",
                principalTable: "SpacedRepetitionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoteCollections_Users_UserId",
                table: "NoteCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteNoteCollection_NoteCollections_NoteCollectionsId",
                table: "NoteNoteCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteNoteCollection_Notes_NotesId",
                table: "NoteNoteCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteSpacedRepetitionGroup_Notes_NotesId",
                table: "NoteSpacedRepetitionGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteSpacedRepetitionGroup_SpacedRepetitionGroups_SpacedRepe~",
                table: "NoteSpacedRepetitionGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteCollections_Users_UserId",
                table: "QuoteCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteQuoteCollection_QuoteCollections_QuoteCollectionsId",
                table: "QuoteQuoteCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteQuoteCollection_Quotes_QuotesId",
                table: "QuoteQuoteCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteSpacedRepetitionGroup_Quotes_QuotesId",
                table: "QuoteSpacedRepetitionGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_QuoteSpacedRepetitionGroup_SpacedRepetitionGroups_SpacedRep~",
                table: "QuoteSpacedRepetitionGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuoteSpacedRepetitionGroup",
                table: "QuoteSpacedRepetitionGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuoteQuoteCollection",
                table: "QuoteQuoteCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuoteCollections",
                table: "QuoteCollections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteSpacedRepetitionGroup",
                table: "NoteSpacedRepetitionGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteNoteCollection",
                table: "NoteNoteCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteCollections",
                table: "NoteCollections");

            migrationBuilder.RenameTable(
                name: "QuoteSpacedRepetitionGroup",
                newName: "QuotesSpacedRepetition");

            migrationBuilder.RenameTable(
                name: "QuoteQuoteCollection",
                newName: "CollectionQuotes");

            migrationBuilder.RenameTable(
                name: "QuoteCollections",
                newName: "QuoteCollection");

            migrationBuilder.RenameTable(
                name: "NoteSpacedRepetitionGroup",
                newName: "NotesSpacedRepetition");

            migrationBuilder.RenameTable(
                name: "NoteNoteCollection",
                newName: "CollectionNotes");

            migrationBuilder.RenameTable(
                name: "NoteCollections",
                newName: "NoteCollection");

            migrationBuilder.RenameIndex(
                name: "IX_QuoteSpacedRepetitionGroup_SpacedRepetitionGroupsId",
                table: "QuotesSpacedRepetition",
                newName: "IX_QuotesSpacedRepetition_SpacedRepetitionGroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_QuoteQuoteCollection_QuotesId",
                table: "CollectionQuotes",
                newName: "IX_CollectionQuotes_QuotesId");

            migrationBuilder.RenameIndex(
                name: "IX_QuoteCollections_UserId",
                table: "QuoteCollection",
                newName: "IX_QuoteCollection_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteSpacedRepetitionGroup_SpacedRepetitionGroupsId",
                table: "NotesSpacedRepetition",
                newName: "IX_NotesSpacedRepetition_SpacedRepetitionGroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteNoteCollection_NotesId",
                table: "CollectionNotes",
                newName: "IX_CollectionNotes_NotesId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteCollections_UserId",
                table: "NoteCollection",
                newName: "IX_NoteCollection_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuotesSpacedRepetition",
                table: "QuotesSpacedRepetition",
                columns: new[] { "QuotesId", "SpacedRepetitionGroupsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollectionQuotes",
                table: "CollectionQuotes",
                columns: new[] { "QuoteCollectionsId", "QuotesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuoteCollection",
                table: "QuoteCollection",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotesSpacedRepetition",
                table: "NotesSpacedRepetition",
                columns: new[] { "NotesId", "SpacedRepetitionGroupsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollectionNotes",
                table: "CollectionNotes",
                columns: new[] { "NoteCollectionsId", "NotesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteCollection",
                table: "NoteCollection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionNotes_NoteCollection_NoteCollectionsId",
                table: "CollectionNotes",
                column: "NoteCollectionsId",
                principalTable: "NoteCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionNotes_Notes_NotesId",
                table: "CollectionNotes",
                column: "NotesId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionQuotes_QuoteCollection_QuoteCollectionsId",
                table: "CollectionQuotes",
                column: "QuoteCollectionsId",
                principalTable: "QuoteCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionQuotes_Quotes_QuotesId",
                table: "CollectionQuotes",
                column: "QuotesId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoteCollection_Users_UserId",
                table: "NoteCollection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotesSpacedRepetition_Notes_NotesId",
                table: "NotesSpacedRepetition",
                column: "NotesId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotesSpacedRepetition_SpacedRepetitionGroups_SpacedRepetiti~",
                table: "NotesSpacedRepetition",
                column: "SpacedRepetitionGroupsId",
                principalTable: "SpacedRepetitionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuoteCollection_Users_UserId",
                table: "QuoteCollection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotesSpacedRepetition_Quotes_QuotesId",
                table: "QuotesSpacedRepetition",
                column: "QuotesId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotesSpacedRepetition_SpacedRepetitionGroups_SpacedRepetit~",
                table: "QuotesSpacedRepetition",
                column: "SpacedRepetitionGroupsId",
                principalTable: "SpacedRepetitionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
