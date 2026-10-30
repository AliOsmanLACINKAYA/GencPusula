using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Hafta2FirsatAlanlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArananBolum",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "ArananSinif",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "GuncellikEtiketi",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "MinimumNotOrtalamasi",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "OnaylandiMi",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "SonKontrolTarihi",
                table: "Firsatlar");

            migrationBuilder.RenameColumn(
                name: "SonTarih",
                table: "Firsatlar",
                newName: "SonBasvuruTarihi");

            migrationBuilder.AlterColumn<string>(
                name: "KaynakBaglantisi",
                table: "Firsatlar",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SonBasvuruTarihi",
                table: "Firsatlar",
                newName: "SonTarih");

            migrationBuilder.AlterColumn<string>(
                name: "KaynakBaglantisi",
                table: "Firsatlar",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "ArananBolum",
                table: "Firsatlar",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArananSinif",
                table: "Firsatlar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuncellikEtiketi",
                table: "Firsatlar",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumNotOrtalamasi",
                table: "Firsatlar",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnaylandiMi",
                table: "Firsatlar",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SonKontrolTarihi",
                table: "Firsatlar",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
