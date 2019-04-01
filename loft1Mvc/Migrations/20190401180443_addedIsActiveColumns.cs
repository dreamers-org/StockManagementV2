using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagement.Migrations
{
    public partial class addedIsActiveColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "XXS",
                table: "Articolo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "XS",
                table: "Articolo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "XL",
                table: "Articolo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "S",
                table: "Articolo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "M",
                table: "Articolo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "L",
                table: "Articolo",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<bool>(
                name: "isLActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isMActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTagliaUnicaActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isXlActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isXsActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isXxlActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isXxsActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isXxxlActive",
                table: "Articolo",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isMActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isSActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isTagliaUnicaActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isXlActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isXsActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isXxlActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isXxsActive",
                table: "Articolo");

            migrationBuilder.DropColumn(
                name: "isXxxlActive",
                table: "Articolo");

            migrationBuilder.AlterColumn<bool>(
                name: "XXS",
                table: "Articolo",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "XS",
                table: "Articolo",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "XL",
                table: "Articolo",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "S",
                table: "Articolo",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "M",
                table: "Articolo",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "L",
                table: "Articolo",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
