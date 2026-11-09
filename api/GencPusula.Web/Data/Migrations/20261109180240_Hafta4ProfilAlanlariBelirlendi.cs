using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Hafta4ProfilAlanlariBelirlendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotOrtalamasi",
                table: "KullaniciProfilleri",
                newName: "GenelNotOrtalamasi");

            migrationBuilder.AlterColumn<string>(
                name: "Sinif",
                table: "KullaniciProfilleri",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "IlgiAlanlari",
                table: "KullaniciProfilleri",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdSoyad",
                table: "KullaniciProfilleri",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "GuncellenmeTarihi",
                table: "KullaniciProfilleri",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Sehir",
                table: "KullaniciProfilleri",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TercihEdilenTurler",
                table: "KullaniciProfilleri",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YabanciDilSeviyesi",
                table: "KullaniciProfilleri",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdSoyad",
                table: "KullaniciProfilleri");

            migrationBuilder.DropColumn(
                name: "GuncellenmeTarihi",
                table: "KullaniciProfilleri");

            migrationBuilder.DropColumn(
                name: "Sehir",
                table: "KullaniciProfilleri");

            migrationBuilder.DropColumn(
                name: "TercihEdilenTurler",
                table: "KullaniciProfilleri");

            migrationBuilder.DropColumn(
                name: "YabanciDilSeviyesi",
                table: "KullaniciProfilleri");

            migrationBuilder.RenameColumn(
                name: "GenelNotOrtalamasi",
                table: "KullaniciProfilleri",
                newName: "NotOrtalamasi");

            migrationBuilder.AlterColumn<int>(
                name: "Sinif",
                table: "KullaniciProfilleri",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "IlgiAlanlari",
                table: "KullaniciProfilleri",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
