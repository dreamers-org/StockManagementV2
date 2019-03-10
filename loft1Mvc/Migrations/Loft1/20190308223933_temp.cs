using Microsoft.EntityFrameworkCore.Migrations;

namespace loft1Mvc.Migrations.Loft1
{
    public partial class temp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genere",
                table: "Articoli",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genere",
                table: "Articoli");
        }
    }
}
