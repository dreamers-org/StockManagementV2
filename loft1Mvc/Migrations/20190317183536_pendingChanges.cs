using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagement.Migrations
{
    public partial class pendingChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackingList_Articolo_IdArticoloId",
                table: "PackingList");

            migrationBuilder.DropIndex(
                name: "IX_PackingList_IdArticoloId",
                table: "PackingList");

            migrationBuilder.DropColumn(
                name: "IdArticoloId",
                table: "PackingList");

            migrationBuilder.AddColumn<Guid>(
                name: "IdArticolo",
                table: "PackingList",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PackingList_Articolo_IdArticoloId",
                table: "PackingList",
                column: "IdArticolo",
                principalTable: "Articolo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdArticolo",
                table: "PackingList");

            migrationBuilder.AddColumn<Guid>(
                name: "IdArticoloId",
                table: "PackingList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackingList_IdArticoloId",
                table: "PackingList",
                column: "IdArticoloId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackingList_Articolo_IdArticoloId",
                table: "PackingList",
                column: "IdArticoloId",
                principalTable: "Articolo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
