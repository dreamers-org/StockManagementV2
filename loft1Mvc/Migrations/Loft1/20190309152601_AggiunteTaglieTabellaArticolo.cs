using Microsoft.EntityFrameworkCore.Migrations;

namespace loft1Mvc.Migrations.Loft1
{
    public partial class AggiunteTaglieTabellaArticolo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attr1",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "attr2",
                table: "Articoli");

            migrationBuilder.AddColumn<bool>(
                name: "L",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "M",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "S",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xl",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xs",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xxl",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xxs",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xxxl",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xxxs",
                table: "Articoli",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Xxxxl",
                table: "Articoli",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "L",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "M",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "S",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xl",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xs",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xxl",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xxs",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xxxl",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xxxs",
                table: "Articoli");

            migrationBuilder.DropColumn(
                name: "Xxxxl",
                table: "Articoli");

            migrationBuilder.AddColumn<string>(
                name: "attr1",
                table: "Articoli",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attr2",
                table: "Articoli",
                nullable: true);
        }
    }
}
