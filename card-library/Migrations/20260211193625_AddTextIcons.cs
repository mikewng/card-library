using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace card_library.Migrations
{
    /// <inheritdoc />
    public partial class AddTextIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextIcons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ImageRefUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextIcons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TextIcons_Name",
                table: "TextIcons",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextIcons");
        }
    }
}
