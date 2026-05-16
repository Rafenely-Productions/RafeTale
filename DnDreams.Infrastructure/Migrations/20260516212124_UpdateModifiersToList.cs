using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModifiersToList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiersJson",
                table: "ItemTemplates");

            migrationBuilder.DropColumn(
                name: "ModifiersJson",
                table: "Feats");

            migrationBuilder.RenameColumn(
                name: "ModifiersJson",
                table: "Features",
                newName: "Modifiers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Modifiers",
                table: "Features",
                newName: "ModifiersJson");

            migrationBuilder.AddColumn<string>(
                name: "ModifiersJson",
                table: "ItemTemplates",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModifiersJson",
                table: "Feats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
