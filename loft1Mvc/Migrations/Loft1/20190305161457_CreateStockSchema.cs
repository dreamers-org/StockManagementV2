using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace loft1Mvc.Migrations.Loft1
{
    public partial class CreateStockSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articoli",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Fornitore = table.Column<string>(maxLength: 50, nullable: false),
                    TipoProdotto = table.Column<string>(nullable: true),
                    Codice = table.Column<int>(maxLength: 50, nullable: false),
                    Descrizione = table.Column<string>(maxLength: 50, nullable: false),
                    Colore = table.Column<string>(maxLength: 50, nullable: false),
                    TrancheConsegna = table.Column<string>(nullable: false),
                    PrezzoAcquisto = table.Column<double>(nullable: false),
                    PrezzoVendita = table.Column<double>(nullable: false),
                    Annullato = table.Column<bool>(nullable: false),
                    attr1 = table.Column<string>(nullable: true),
                    attr2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articoli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticoliAnnullati",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Codice = table.Column<int>(nullable: false),
                    Colore = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticoliAnnullati", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdiniAiFornitori",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdOrdine = table.Column<int>(nullable: false),
                    Fornitore = table.Column<string>(maxLength: 50, nullable: false),
                    Codice = table.Column<int>(nullable: false),
                    Descrizione = table.Column<string>(maxLength: 50, nullable: false),
                    Colore = table.Column<string>(maxLength: 50, nullable: false),
                    PrezzoAcquisto = table.Column<double>(nullable: false),
                    PrezzoVendita = table.Column<double>(nullable: false),
                    xxxs = table.Column<int>(nullable: true),
                    xxs = table.Column<int>(nullable: true),
                    xs = table.Column<int>(nullable: true),
                    s = table.Column<int>(nullable: true),
                    m = table.Column<int>(nullable: true),
                    l = table.Column<int>(nullable: true),
                    xl = table.Column<int>(nullable: true),
                    xxl = table.Column<int>(nullable: true),
                    xxxl = table.Column<int>(nullable: true),
                    xxxxl = table.Column<int>(nullable: true),
                    attr1 = table.Column<string>(nullable: true),
                    attr2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdiniAiFornitori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdiniDaiClienti",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdOrdine = table.Column<int>(nullable: false),
                    Cliente = table.Column<string>(maxLength: 50, nullable: false),
                    Rappresentante = table.Column<string>(maxLength: 50, nullable: false),
                    DataOrdine = table.Column<DateTime>(type: "date", nullable: false),
                    DataConsegna = table.Column<DateTime>(type: "date", nullable: false),
                    Indirizzo = table.Column<string>(maxLength: 50, nullable: true),
                    Pagamento = table.Column<string>(maxLength: 50, nullable: false),
                    Codice = table.Column<int>(nullable: false),
                    Colore = table.Column<string>(maxLength: 50, nullable: false),
                    xxxs = table.Column<int>(nullable: true),
                    xxs = table.Column<int>(nullable: true),
                    xs = table.Column<int>(nullable: true),
                    s = table.Column<int>(nullable: true),
                    m = table.Column<int>(nullable: true),
                    l = table.Column<int>(nullable: true),
                    xl = table.Column<int>(nullable: true),
                    xxl = table.Column<int>(nullable: true),
                    xxxl = table.Column<int>(nullable: true),
                    xxxxl = table.Column<int>(nullable: true),
                    attr1 = table.Column<string>(nullable: true),
                    attr2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdiniDaiClienti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackingList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Codice = table.Column<int>(nullable: false),
                    Fornitore = table.Column<string>(maxLength: 50, nullable: false),
                    Variante = table.Column<string>(maxLength: 50, nullable: false),
                    xxxs = table.Column<int>(nullable: true),
                    xxs = table.Column<int>(nullable: true),
                    xs = table.Column<int>(nullable: true),
                    s = table.Column<int>(nullable: true),
                    m = table.Column<int>(nullable: true),
                    l = table.Column<int>(nullable: true),
                    xl = table.Column<int>(nullable: true),
                    xxl = table.Column<int>(nullable: true),
                    xxxl = table.Column<int>(nullable: true),
                    xxxxl = table.Column<int>(nullable: true),
                    attr1 = table.Column<string>(nullable: true),
                    attr2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoProdotto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tipo = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProdotto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrancheConsegna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataConsegna = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrancheConsegna", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articoli");

            migrationBuilder.DropTable(
                name: "ArticoliAnnullati");

            migrationBuilder.DropTable(
                name: "OrdiniAiFornitori");

            migrationBuilder.DropTable(
                name: "OrdiniDaiClienti");

            migrationBuilder.DropTable(
                name: "PackingList");

            migrationBuilder.DropTable(
                name: "TipoProdotto");

            migrationBuilder.DropTable(
                name: "TrancheConsegna");
        }
    }
}
