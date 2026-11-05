using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciProfiliAdlandirmasi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasvuruKayitlari_OgrenciProfilleri_OgrenciProfiliId",
                table: "BasvuruKayitlari");

            migrationBuilder.RenameTable(
                name: "OgrenciProfilleri",
                newName: "KullaniciProfilleri");

            migrationBuilder.Sql(
                "ALTER TABLE \"KullaniciProfilleri\" " +
                "RENAME CONSTRAINT \"PK_OgrenciProfilleri\" TO \"PK_KullaniciProfilleri\";");

            migrationBuilder.Sql(
                "ALTER SEQUENCE \"OgrenciProfilleri_Id_seq\" " +
                "RENAME TO \"KullaniciProfilleri_Id_seq\";");

            migrationBuilder.RenameColumn(
                name: "OgrenciProfiliId",
                table: "BasvuruKayitlari",
                newName: "KullaniciProfiliId");

            migrationBuilder.RenameIndex(
                name: "IX_BasvuruKayitlari_OgrenciProfiliId",
                table: "BasvuruKayitlari",
                newName: "IX_BasvuruKayitlari_KullaniciProfiliId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasvuruKayitlari_KullaniciProfilleri_KullaniciProfiliId",
                table: "BasvuruKayitlari",
                column: "KullaniciProfiliId",
                principalTable: "KullaniciProfilleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasvuruKayitlari_KullaniciProfilleri_KullaniciProfiliId",
                table: "BasvuruKayitlari");

            migrationBuilder.RenameTable(
                name: "KullaniciProfilleri",
                newName: "OgrenciProfilleri");

            migrationBuilder.Sql(
                "ALTER TABLE \"OgrenciProfilleri\" " +
                "RENAME CONSTRAINT \"PK_KullaniciProfilleri\" TO \"PK_OgrenciProfilleri\";");

            migrationBuilder.Sql(
                "ALTER SEQUENCE \"KullaniciProfilleri_Id_seq\" " +
                "RENAME TO \"OgrenciProfilleri_Id_seq\";");

            migrationBuilder.RenameColumn(
                name: "KullaniciProfiliId",
                table: "BasvuruKayitlari",
                newName: "OgrenciProfiliId");

            migrationBuilder.RenameIndex(
                name: "IX_BasvuruKayitlari_KullaniciProfiliId",
                table: "BasvuruKayitlari",
                newName: "IX_BasvuruKayitlari_OgrenciProfiliId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasvuruKayitlari_OgrenciProfilleri_OgrenciProfiliId",
                table: "BasvuruKayitlari",
                column: "OgrenciProfiliId",
                principalTable: "OgrenciProfilleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
