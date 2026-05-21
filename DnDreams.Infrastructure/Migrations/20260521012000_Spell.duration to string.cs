using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Spelldurationtostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Duration",
                table: "Spells",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Languages",
                table: "Races",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "SpecialData",
                table: "Features",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArmorProficiencies",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryAbility",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SavingThrowProficiencies",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkillProficiencies",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SkillsToChoose",
                table: "ClassDefinitions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WeaponProficiencies",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialData",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "ArmorProficiencies",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "PrimaryAbility",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "SavingThrowProficiencies",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "SkillProficiencies",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "SkillsToChoose",
                table: "ClassDefinitions");

            migrationBuilder.DropColumn(
                name: "WeaponProficiencies",
                table: "ClassDefinitions");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Spells",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Languages",
                table: "Races",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
