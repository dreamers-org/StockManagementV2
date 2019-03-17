using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagement.Migrations
{
    public partial class CreataTabellaPackingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "PackingList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    IdArticoloId = table.Column<Guid>(nullable: true),
                    Xxs = table.Column<int>(nullable: false),
                    Xs = table.Column<int>(nullable: false),
                    S = table.Column<int>(nullable: false),
                    M = table.Column<int>(nullable: false),
                    L = table.Column<int>(nullable: false),
                    Xl = table.Column<int>(nullable: false),
                    Xxl = table.Column<int>(nullable: false),
                    Xxxl = table.Column<int>(nullable: false),
                    TagliaUnica = table.Column<int>(nullable: false),
                    DataInserimento = table.Column<DateTime>(nullable: false),
                    UtenteInserimento = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingList_Articolo_IdArticoloId",
                        column: x => x.IdArticoloId,
                        principalTable: "Articolo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackingList_IdArticoloId",
                table: "PackingList",
                column: "IdArticoloId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackingList");
        }
    }
}
