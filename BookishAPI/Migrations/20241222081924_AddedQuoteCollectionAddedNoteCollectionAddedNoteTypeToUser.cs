using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedQuoteCollectionAddedNoteCollectionAddedNoteTypeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "NoteTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "NoteCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteCollection_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    NoteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteImage_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteCollection_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionNotes",
                columns: table => new
                {
                    NoteCollectionsId = table.Column<int>(type: "integer", nullable: false),
                    NotesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionNotes", x => new { x.NoteCollectionsId, x.NotesId });
                    table.ForeignKey(
                        name: "FK_CollectionNotes_NoteCollection_NoteCollectionsId",
                        column: x => x.NoteCollectionsId,
                        principalTable: "NoteCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionNotes_Notes_NotesId",
                        column: x => x.NotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionQuotes",
                columns: table => new
                {
                    QuoteCollectionsId = table.Column<int>(type: "integer", nullable: false),
                    QuotesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionQuotes", x => new { x.QuoteCollectionsId, x.QuotesId });
                    table.ForeignKey(
                        name: "FK_CollectionQuotes_QuoteCollection_QuoteCollectionsId",
                        column: x => x.QuoteCollectionsId,
                        principalTable: "QuoteCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionQuotes_Quotes_QuotesId",
                        column: x => x.QuotesId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NoteTypes_UserId",
                table: "NoteTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionNotes_NotesId",
                table: "CollectionNotes",
                column: "NotesId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionQuotes_QuotesId",
                table: "CollectionQuotes",
                column: "QuotesId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteCollection_UserId",
                table: "NoteCollection",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteImage_NoteId",
                table: "NoteImage",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteCollection_UserId",
                table: "QuoteCollection",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NoteTypes_Users_UserId",
                table: "NoteTypes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoteTypes_Users_UserId",
                table: "NoteTypes");

            migrationBuilder.DropTable(
                name: "CollectionNotes");

            migrationBuilder.DropTable(
                name: "CollectionQuotes");

            migrationBuilder.DropTable(
                name: "NoteImage");

            migrationBuilder.DropTable(
                name: "NoteCollection");

            migrationBuilder.DropTable(
                name: "QuoteCollection");

            migrationBuilder.DropIndex(
                name: "IX_NoteTypes_UserId",
                table: "NoteTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NoteTypes");
        }
    }
}
