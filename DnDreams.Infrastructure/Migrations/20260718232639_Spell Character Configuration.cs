using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpellCharacterConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassDefinitionSpell");

            migrationBuilder.AddColumn<string>(
                name: "ClassesTechnicalNames",
                table: "Spells",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassesTechnicalNames",
                table: "Spells");

            migrationBuilder.CreateTable(
                name: "ClassDefinitionSpell",
                columns: table => new
                {
                    ClassesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpellId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassDefinitionSpell", x => new { x.ClassesId, x.SpellId });
                    table.ForeignKey(
                        name: "FK_ClassDefinitionSpell_ClassDefinitions_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "ClassDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassDefinitionSpell_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassDefinitionSpell_SpellId",
                table: "ClassDefinitionSpell",
                column: "SpellId");
        }
    }
}
