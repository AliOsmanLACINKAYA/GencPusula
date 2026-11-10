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

            migrationBuilder.Sql(
                "UPDATE \"KullaniciProfilleri\" SET \"Sinif\" = CASE \"Sinif\" " +
                "WHEN '1' THEN '1. Sınıf' WHEN '2' THEN '2. Sınıf' " +
                "WHEN '3' THEN '3. Sınıf' WHEN '4' THEN '4. Sınıf' " +
                "WHEN '5' THEN '5. Sınıf' WHEN '6' THEN '6. Sınıf' " +
                "ELSE \"Sinif\" END;");

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

            migrationBuilder.Sql(
                "UPDATE \"KullaniciProfilleri\" SET \"GuncellenmeTarihi\" = NOW() " +
                "WHERE \"GuncellenmeTarihi\" = '-infinity';");

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

            // Mükerrer profillere bağlı başvuruları en eski profile taşı; kayıt kaybını önle.
            migrationBuilder.Sql(
                "UPDATE \"BasvuruKayitlari\" b SET \"KullaniciProfiliId\" = (" +
                "SELECT MIN(k.\"Id\") FROM \"KullaniciProfilleri\" p " +
                "JOIN \"KullaniciProfilleri\" k ON k.\"KullaniciId\" = p.\"KullaniciId\" " +
                "WHERE p.\"Id\" = b.\"KullaniciProfiliId\") " +
                "WHERE EXISTS (SELECT 1 FROM \"KullaniciProfilleri\" p " +
                "JOIN \"KullaniciProfilleri\" k ON k.\"KullaniciId\" = p.\"KullaniciId\" " +
                "AND k.\"Id\" < p.\"Id\" WHERE p.\"Id\" = b.\"KullaniciProfiliId\");");

            migrationBuilder.Sql(
                "DELETE FROM \"KullaniciProfilleri\" p USING \"KullaniciProfilleri\" q " +
                "WHERE p.\"Id\" > q.\"Id\" AND p.\"KullaniciId\" = q.\"KullaniciId\";");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciProfilleri_KullaniciId",
                table: "KullaniciProfilleri",
                column: "KullaniciId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KullaniciProfilleri_KullaniciId",
                table: "KullaniciProfilleri");

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

            migrationBuilder.Sql(
                "UPDATE \"KullaniciProfilleri\" SET \"Sinif\" = CASE " +
                "WHEN \"Sinif\" = '1. Sınıf' THEN '1' WHEN \"Sinif\" = '2. Sınıf' THEN '2' " +
                "WHEN \"Sinif\" = '3. Sınıf' THEN '3' WHEN \"Sinif\" = '4. Sınıf' THEN '4' " +
                "WHEN \"Sinif\" = '5. Sınıf' THEN '5' WHEN \"Sinif\" = '6. Sınıf' THEN '6' " +
                "ELSE '1' END;");

            migrationBuilder.Sql(
                "ALTER TABLE \"KullaniciProfilleri\" " +
                "ALTER COLUMN \"Sinif\" TYPE integer USING \"Sinif\"::integer;");

            migrationBuilder.AlterColumn<string>(
                name: "IlgiAlanlari",
                table: "KullaniciProfilleri",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.Sql(
                "ALTER TABLE \"KullaniciProfilleri\" " +
                "ALTER COLUMN \"IlgiAlanlari\" DROP DEFAULT;");
        }
    }
}
