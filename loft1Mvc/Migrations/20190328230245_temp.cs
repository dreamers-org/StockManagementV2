using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagement.Migrations
{
    public partial class temp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Stampato",
                table: "OrdineCliente",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SpeditoInParte",
                table: "OrdineCliente",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Spedito",
                table: "OrdineCliente",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Letto",
                table: "OrdineCliente",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Stampato",
                table: "OrdineCliente",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SpeditoInParte",
                table: "OrdineCliente",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Spedito",
                table: "OrdineCliente",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Letto",
                table: "OrdineCliente",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
