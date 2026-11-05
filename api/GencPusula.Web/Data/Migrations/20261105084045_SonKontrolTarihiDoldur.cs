using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SonKontrolTarihiDoldur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Firsatlar\" " +
                "SET \"SonKontrolTarihi\" = NOW() " +
                "WHERE \"SonKontrolTarihi\" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Tek seferlik veri doldurma; geri alınacak bir şema değişikliği yok.
        }
    }
}
