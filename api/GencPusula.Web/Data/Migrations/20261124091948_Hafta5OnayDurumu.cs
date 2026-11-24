using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GencPusula.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Hafta5OnayDurumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnayDurumu",
                table: "Firsatlar",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Onaylı");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnayDurumu",
                table: "Firsatlar");
        }
    }
}
