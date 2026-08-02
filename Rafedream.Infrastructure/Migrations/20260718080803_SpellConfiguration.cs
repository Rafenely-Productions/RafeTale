using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rafedream.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpellConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassDefinitions_Spells_SpellId",
                table: "ClassDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_ClassDefinitions_SpellId",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "SpellId",
                table: "ClassDefinitions");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassDefinitionSpell");

            migrationBuilder.AddColumn<Guid>(
                name: "SpellId",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassDefinitions_SpellId",
                table: "ClassDefinitions",
                column: "SpellId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassDefinitions_Spells_SpellId",
                table: "ClassDefinitions",
                column: "SpellId",
                principalTable: "Spells",
                principalColumn: "Id");
        }
    }
}
