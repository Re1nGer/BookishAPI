using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookishAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedInterestsBooksAndPurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterestArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadingPurpose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingPurpose", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelectedBook",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedBook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestAreaUser",
                columns: table => new
                {
                    InterestAreasId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestAreaUser", x => new { x.InterestAreasId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_InterestAreaUser_InterestArea_InterestAreasId",
                        column: x => x.InterestAreasId,
                        principalTable: "InterestArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestAreaUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadingPurposeUser",
                columns: table => new
                {
                    ReadingPurposesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingPurposeUser", x => new { x.ReadingPurposesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ReadingPurposeUser_ReadingPurpose_ReadingPurposesId",
                        column: x => x.ReadingPurposesId,
                        principalTable: "ReadingPurpose",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReadingPurposeUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedBookUser",
                columns: table => new
                {
                    SelectedBooksId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedBookUser", x => new { x.SelectedBooksId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SelectedBookUser_SelectedBook_SelectedBooksId",
                        column: x => x.SelectedBooksId,
                        principalTable: "SelectedBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedBookUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterestAreaUser_UsersId",
                table: "InterestAreaUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingPurposeUser_UsersId",
                table: "ReadingPurposeUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedBookUser_UsersId",
                table: "SelectedBookUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterestAreaUser");

            migrationBuilder.DropTable(
                name: "ReadingPurposeUser");

            migrationBuilder.DropTable(
                name: "SelectedBookUser");

            migrationBuilder.DropTable(
                name: "InterestArea");

            migrationBuilder.DropTable(
                name: "ReadingPurpose");

            migrationBuilder.DropTable(
                name: "SelectedBook");
        }
    }
}
