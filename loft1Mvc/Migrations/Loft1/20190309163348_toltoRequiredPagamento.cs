﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace loft1Mvc.Migrations.Loft1
{
    public partial class toltoRequiredPagamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdOrdine",
                table: "OrdiniDaiClienti",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdOrdine",
                table: "OrdiniDaiClienti",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
