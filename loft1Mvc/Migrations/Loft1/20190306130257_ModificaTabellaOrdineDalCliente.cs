using Microsoft.EntityFrameworkCore.Migrations;

namespace loft1Mvc.Migrations.Loft1
{
    public partial class ModificaTabellaOrdineDalCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descrizione",
                table: "OrdiniDaiClienti",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Descrizione",
                table: "OrdiniDaiClienti",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
