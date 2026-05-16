using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ForjandoEstructuraCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelProgression_Characters_CharacterId",
                table: "ClassLevelProgression");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelProgression_ClassDefinitions_ClassDefinitionId",
                table: "ClassLevelProgression");

            migrationBuilder.DropForeignKey(
                name: "FK_Features_Characters_CharacterId",
                table: "Features");

            migrationBuilder.DropForeignKey(
                name: "FK_Features_ClassLevelProgression_ClassLevelProgressionId",
                table: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Features_CharacterId",
                table: "Features");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassLevelProgression",
                table: "ClassLevelProgression");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelProgression_ClassDefinitionId",
                table: "ClassLevelProgression");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ClassDefinitionId",
                table: "ClassLevelProgression");

            migrationBuilder.RenameTable(
                name: "ClassLevelProgression",
                newName: "ClassLevelProgressions");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevelProgression_CharacterId",
                table: "ClassLevelProgressions",
                newName: "IX_ClassLevelProgressions_CharacterId");

            migrationBuilder.AddColumn<string>(
                name: "ModifiersJson",
                table: "Features",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassDefId",
                table: "ClassLevelProgressions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassLevelProgressions",
                table: "ClassLevelProgressions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CharacterFeature",
                columns: table => new
                {
                    AcquiredFeaturesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterFeature", x => new { x.AcquiredFeaturesId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_CharacterFeature_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterFeature_Features_AcquiredFeaturesId",
                        column: x => x.AcquiredFeaturesId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "characterModifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Target = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characterModifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_characterModifiers_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Prerequisite = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiersJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    GoldValue = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageDice = table.Column<string>(type: "TEXT", nullable: true),
                    ArmorClass = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiersJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spells",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    School = table.Column<string>(type: "TEXT", nullable: false),
                    CastingTime = table.Column<string>(type: "TEXT", nullable: false),
                    Range = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XpRules",
                columns: table => new
                {
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RequiredXp = table.Column<int>(type: "INTEGER", nullable: false),
                    Bonus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XpRules", x => x.Level);
                });

            migrationBuilder.CreateTable(
                name: "CharacterFeat",
                columns: table => new
                {
                    AcquiredFeatsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterFeat", x => new { x.AcquiredFeatsId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_CharacterFeat_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterFeat_Feats_AcquiredFeatsId",
                        column: x => x.AcquiredFeatsId,
                        principalTable: "Feats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEquipped = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInventories_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterInventories_ItemTemplates_ItemTemplateId",
                        column: x => x.ItemTemplateId,
                        principalTable: "ItemTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSpell",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "TEXT", nullable: false),
                    KnownSpellsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSpell", x => new { x.CharacterId, x.KnownSpellsId });
                    table.ForeignKey(
                        name: "FK_CharacterSpell_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterSpell_Spells_KnownSpellsId",
                        column: x => x.KnownSpellsId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelProgressions_ClassDefId",
                table: "ClassLevelProgressions",
                column: "ClassDefId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterFeat_CharacterId",
                table: "CharacterFeat",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterFeature_CharacterId",
                table: "CharacterFeature",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventories_CharacterId",
                table: "CharacterInventories",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventories_ItemTemplateId",
                table: "CharacterInventories",
                column: "ItemTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_characterModifiers_CharacterId",
                table: "characterModifiers",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpell_KnownSpellsId",
                table: "CharacterSpell",
                column: "KnownSpellsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelProgressions_Characters_CharacterId",
                table: "ClassLevelProgressions",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelProgressions_ClassDefinitions_ClassDefId",
                table: "ClassLevelProgressions",
                column: "ClassDefId",
                principalTable: "ClassDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Features_ClassLevelProgressions_ClassLevelProgressionId",
                table: "Features",
                column: "ClassLevelProgressionId",
                principalTable: "ClassLevelProgressions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelProgressions_Characters_CharacterId",
                table: "ClassLevelProgressions");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelProgressions_ClassDefinitions_ClassDefId",
                table: "ClassLevelProgressions");

            migrationBuilder.DropForeignKey(
                name: "FK_Features_ClassLevelProgressions_ClassLevelProgressionId",
                table: "Features");

            migrationBuilder.DropTable(
                name: "CharacterFeat");

            migrationBuilder.DropTable(
                name: "CharacterFeature");

            migrationBuilder.DropTable(
                name: "CharacterInventories");

            migrationBuilder.DropTable(
                name: "characterModifiers");

            migrationBuilder.DropTable(
                name: "CharacterSpell");

            migrationBuilder.DropTable(
                name: "XpRules");

            migrationBuilder.DropTable(
                name: "Feats");

            migrationBuilder.DropTable(
                name: "ItemTemplates");

            migrationBuilder.DropTable(
                name: "Spells");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassLevelProgressions",
                table: "ClassLevelProgressions");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelProgressions_ClassDefId",
                table: "ClassLevelProgressions");

            migrationBuilder.DropColumn(
                name: "ModifiersJson",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "ClassDefId",
                table: "ClassLevelProgressions");

            migrationBuilder.RenameTable(
                name: "ClassLevelProgressions",
                newName: "ClassLevelProgression");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevelProgressions_CharacterId",
                table: "ClassLevelProgression",
                newName: "IX_ClassLevelProgression_CharacterId");

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                table: "Features",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "Characters",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ClassDefinitionId",
                table: "ClassLevelProgression",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassLevelProgression",
                table: "ClassLevelProgression",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Features_CharacterId",
                table: "Features",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelProgression_ClassDefinitionId",
                table: "ClassLevelProgression",
                column: "ClassDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelProgression_Characters_CharacterId",
                table: "ClassLevelProgression",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelProgression_ClassDefinitions_ClassDefinitionId",
                table: "ClassLevelProgression",
                column: "ClassDefinitionId",
                principalTable: "ClassDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Characters_CharacterId",
                table: "Features",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_ClassLevelProgression_ClassLevelProgressionId",
                table: "Features",
                column: "ClassLevelProgressionId",
                principalTable: "ClassLevelProgression",
                principalColumn: "Id");
        }
    }
}
