using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTraitsAndRaceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Races",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Trait",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    RaceId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trait", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trait_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trait_RaceId",
                table: "Trait",
                column: "RaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trait");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Races");
        }
    }
}
