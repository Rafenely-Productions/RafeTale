using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToRaceAndClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Races",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ClassDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ClassDefinitions");
        }
    }
}
