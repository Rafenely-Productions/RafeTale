using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnDreams.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RacesSubracestraitsfixes : Migration
    {
        public RacesSubracestraitsfixes()
        {
            // Forzamos a que esta migración no use transacciones para que SQLite pueda desactivar las FKeys temporalmente
            ActiveProvider = "Microsoft.EntityFrameworkCore.Sqlite";
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            migrationBuilder.DropColumn(
                name: "Darkvision",
                table: "Races");

            migrationBuilder.AlterColumn<Guid>(
                name: "RaceId",
                table: "Traits",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "SubraceId",
                table: "Traits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Traits_SubraceId",
                table: "Traits",
                column: "SubraceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Traits_SubRaces_SubraceId",
                table: "Traits",
                column: "SubraceId",
                principalTable: "SubRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            migrationBuilder.DropForeignKey(
                name: "FK_Traits_SubRaces_SubraceId",
                table: "Traits");

            migrationBuilder.DropIndex(
                name: "IX_Traits_SubraceId",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "SubraceId",
                table: "Traits");

            migrationBuilder.AlterColumn<Guid>(
                name: "RaceId",
                table: "Traits",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Darkvision",
                table: "Races",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }
    }
}
