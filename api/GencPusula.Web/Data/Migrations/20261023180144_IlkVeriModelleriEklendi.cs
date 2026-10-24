using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class IlkVeriModelleriEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Firsatlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: false),
                    FirsatTuru = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Sehir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SonTarih = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KaynakBaglantisi = table.Column<string>(type: "text", nullable: false),
                    SonKontrolTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellikEtiketi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OnaylandiMi = table.Column<bool>(type: "boolean", nullable: false),
                    ArananBolum = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ArananSinif = table.Column<int>(type: "integer", nullable: true),
                    MinimumNotOrtalamasi = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firsatlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OgrenciProfilleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<string>(type: "text", nullable: false),
                    Bolum = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Sinif = table.Column<int>(type: "integer", nullable: false),
                    NotOrtalamasi = table.Column<decimal>(type: "numeric", nullable: true),
                    IlgiAlanlari = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgrenciProfilleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BasvuruKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirsatId = table.Column<int>(type: "integer", nullable: false),
                    OgrenciProfiliId = table.Column<int>(type: "integer", nullable: false),
                    BasvuruDurumu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KisiselNotlar = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasvuruKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasvuruKayitlari_Firsatlar_FirsatId",
                        column: x => x.FirsatId,
                        principalTable: "Firsatlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasvuruKayitlari_OgrenciProfilleri_OgrenciProfiliId",
                        column: x => x.OgrenciProfiliId,
                        principalTable: "OgrenciProfilleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruKayitlari_FirsatId",
                table: "BasvuruKayitlari",
                column: "FirsatId");

            migrationBuilder.CreateIndex(
                name: "IX_BasvuruKayitlari_OgrenciProfiliId",
                table: "BasvuruKayitlari",
                column: "OgrenciProfiliId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasvuruKayitlari");

            migrationBuilder.DropTable(
                name: "Firsatlar");

            migrationBuilder.DropTable(
                name: "OgrenciProfilleri");
        }
    }
}
