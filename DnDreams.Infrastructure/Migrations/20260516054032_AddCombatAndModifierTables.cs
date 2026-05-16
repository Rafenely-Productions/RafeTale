using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCombatAndModifierTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Target",
                table: "characterModifiers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "ActiveModifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    TargetProperty = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    DurationType = table.Column<string>(type: "TEXT", nullable: false),
                    RemainingRounds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveModifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveModifiers_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSpellSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SlotLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalSlots = table.Column<int>(type: "INTEGER", nullable: false),
                    UsedSlots = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSpellSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterSpellSlots_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentHp = table.Column<int>(type: "INTEGER", nullable: false),
                    TemporaryHp = table.Column<int>(type: "INTEGER", nullable: false),
                    DeathSaveSuccesses = table.Column<int>(type: "INTEGER", nullable: false),
                    DeathSaveFailures = table.Column<int>(type: "INTEGER", nullable: false),
                    ActiveConditions = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterStatuses_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveModifiers_CharacterId",
                table: "ActiveModifiers",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpellSlots_CharacterId",
                table: "CharacterSpellSlots",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatuses_CharacterId",
                table: "CharacterStatuses",
                column: "CharacterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveModifiers");

            migrationBuilder.DropTable(
                name: "CharacterSpellSlots");

            migrationBuilder.DropTable(
                name: "CharacterStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "Target",
                table: "characterModifiers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
