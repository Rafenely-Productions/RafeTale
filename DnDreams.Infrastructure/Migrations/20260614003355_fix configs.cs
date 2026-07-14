using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fixconfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelProgressions_Subclasses_SubclassId",
                table: "ClassLevelProgressions");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelProgressions_SubclassId",
                table: "ClassLevelProgressions");

            migrationBuilder.DropColumn(
                name: "SubclassId",
                table: "ClassLevelProgressions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubclassId",
                table: "ClassLevelProgressions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelProgressions_SubclassId",
                table: "ClassLevelProgressions",
                column: "SubclassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelProgressions_Subclasses_SubclassId",
                table: "ClassLevelProgressions",
                column: "SubclassId",
                principalTable: "Subclasses",
                principalColumn: "Id");
        }
    }
}
