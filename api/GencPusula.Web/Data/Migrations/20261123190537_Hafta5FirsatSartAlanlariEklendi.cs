using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Hafta5FirsatSartAlanlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GerekliBolum",
                table: "Firsatlar",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GerekliDilSeviyesi",
                table: "Firsatlar",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GerekliSinif",
                table: "Firsatlar",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinNotOrtalamasi",
                table: "Firsatlar",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GerekliBolum",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "GerekliDilSeviyesi",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "GerekliSinif",
                table: "Firsatlar");

            migrationBuilder.DropColumn(
                name: "MinNotOrtalamasi",
                table: "Firsatlar");
        }
    }
}
