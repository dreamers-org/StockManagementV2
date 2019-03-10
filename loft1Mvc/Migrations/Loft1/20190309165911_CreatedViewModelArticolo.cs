using Microsoft.EntityFrameworkCore.Migrations;

namespace loft1Mvc.Migrations.Loft1
{
    public partial class CreatedViewModelArticolo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataInserimento",
                table: "Articoli",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Operatore",
                table: "Articoli",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInserimento",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Operatore",
                table: "Articoli");
        }
    }
}
