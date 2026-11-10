using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Hafta4KayitliFirsatIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Unique index öncesi aynı profil + fırsat mükerrerlerinden en eskisini koru.
            migrationBuilder.Sql(
                "DELETE FROM \"BasvuruKayitlari\" b USING \"BasvuruKayitlari\" c " +
                "WHERE b.\"Id\" > c.\"Id\" " +
                "AND b.\"KullaniciProfiliId\" = c.\"KullaniciProfiliId\" " +
                "AND b.\"FirsatId\" = c.\"FirsatId\";");

            migrationBuilder.DropIndex(
                name: "IX_BasvuruKayitlari_KullaniciProfiliId",
                table: "BasvuruKayitlari");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruKayitlari_KullaniciProfiliId_FirsatId",
                table: "BasvuruKayitlari",
                columns: new[] { "KullaniciProfiliId", "FirsatId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BasvuruKayitlari_KullaniciProfiliId_FirsatId",
                table: "BasvuruKayitlari");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruKayitlari_KullaniciProfiliId",
                table: "BasvuruKayitlari",
                column: "KullaniciProfiliId");
        }
    }
}
