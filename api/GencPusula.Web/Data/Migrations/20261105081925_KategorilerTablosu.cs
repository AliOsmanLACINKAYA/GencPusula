using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class KategorilerTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Ad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Ad);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Firsatlar_FirsatTuru",
                table: "Firsatlar",
                column: "FirsatTuru");

            migrationBuilder.Sql(
                "INSERT INTO \"Kategoriler\" (\"Ad\") " +
                "SELECT DISTINCT \"FirsatTuru\" FROM \"Firsatlar\";");

            migrationBuilder.AddForeignKey(
                name: "FK_Firsatlar_Kategoriler_FirsatTuru",
                table: "Firsatlar",
                column: "FirsatTuru",
                principalTable: "Kategoriler",
                principalColumn: "Ad",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firsatlar_Kategoriler_FirsatTuru",
                table: "Firsatlar");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Firsatlar_FirsatTuru",
                table: "Firsatlar");
        }
    }
}
