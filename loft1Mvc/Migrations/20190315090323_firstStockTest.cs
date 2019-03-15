using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockManagement.Migrations
{
    public partial class firstStockTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Nome = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    Indirizzo = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    Email = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    Attivo = table.Column<bool>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collezione",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Nome = table.Column<string>(unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collezione", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornitore",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Nome = table.Column<string>(unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornitore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdineFornitore",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    DataInserimento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DataModifica = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtenteInserimento = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    UtenteModifica = table.Column<string>(unicode: false, maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdineFornitore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tipo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Nome = table.Column<string>(unicode: false, maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoPagamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Nome = table.Column<string>(unicode: false, maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoPagamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articolo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    Codice = table.Column<string>(unicode: false, maxLength: 64, nullable: false),
                    Descrizione = table.Column<string>(unicode: false, maxLength: 256, nullable: false),
                    IdFornitore = table.Column<Guid>(nullable: false),
                    Colore = table.Column<string>(unicode: false, maxLength: 64, nullable: false),
                    XXS = table.Column<bool>(nullable: false),
                    XS = table.Column<bool>(nullable: false),
                    S = table.Column<bool>(nullable: false),
                    M = table.Column<bool>(nullable: false),
                    L = table.Column<bool>(nullable: false),
                    XL = table.Column<bool>(nullable: false),
                    XXL = table.Column<bool>(nullable: false),
                    XXXL = table.Column<bool>(nullable: false),
                    TagliaUnica = table.Column<bool>(nullable: false),
                    TrancheConsegna = table.Column<DateTime>(type: "datetime", nullable: false),
                    Genere = table.Column<string>(unicode: false, maxLength: 16, nullable: false),
                    IdTipo = table.Column<Guid>(nullable: false),
                    Annullato = table.Column<bool>(nullable: false),
                    PrezzoAcquisto = table.Column<double>(nullable: false),
                    PrezzoVendita = table.Column<double>(nullable: false),
                    Foto = table.Column<string>(unicode: false, maxLength: 1024, nullable: false),
                    Video = table.Column<string>(unicode: false, maxLength: 1024, nullable: false),
                    IdCollezione = table.Column<Guid>(nullable: false),
                    DataInserimento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DataModifica = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtenteInserimento = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    UtenteModifica = table.Column<string>(unicode: false, maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articolo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articolo_Collezione1",
                        column: x => x.IdCollezione,
                        principalTable: "Collezione",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articolo_Fornitore1",
                        column: x => x.IdFornitore,
                        principalTable: "Fornitore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articolo_Tipo1",
                        column: x => x.IdTipo,
                        principalTable: "Tipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdineCliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    IdRappresentante = table.Column<Guid>(nullable: false),
                    IdCliente = table.Column<Guid>(nullable: false),
                    DataConsegna = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdTipoPagamento = table.Column<Guid>(nullable: true),
                    Note = table.Column<string>(unicode: false, maxLength: 8000, nullable: true),
                    Completato = table.Column<bool>(nullable: false),
                    Pagato = table.Column<bool>(nullable: false),
                    DataInserimento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DataModifica = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtenteInserimento = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    UtenteModifica = table.Column<string>(unicode: false, maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdineCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdineCliente_Cliente",
                        column: x => x.Id,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdineCliente_TipoPagamento",
                        column: x => x.IdTipoPagamento,
                        principalTable: "TipoPagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RigaOrdineFornitore",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    IdOrdine = table.Column<Guid>(nullable: false),
                    IdArticolo = table.Column<Guid>(nullable: false),
                    XXSArrivato = table.Column<int>(nullable: false),
                    XXS = table.Column<int>(nullable: false),
                    XS = table.Column<int>(nullable: false),
                    XSArrivato = table.Column<int>(nullable: false),
                    S = table.Column<int>(nullable: false),
                    SArrivato = table.Column<int>(nullable: false),
                    M = table.Column<int>(nullable: false),
                    MArrivato = table.Column<int>(nullable: false),
                    L = table.Column<int>(nullable: false),
                    LArrivato = table.Column<int>(nullable: false),
                    XL = table.Column<int>(nullable: false),
                    XLArrivato = table.Column<int>(nullable: false),
                    XXL = table.Column<int>(nullable: false),
                    XXLArrivato = table.Column<int>(nullable: false),
                    XXXL = table.Column<int>(nullable: false),
                    XXXLArrivato = table.Column<int>(nullable: false),
                    UtenteModifica = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                    UtenteInserimento = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    DataModifica = table.Column<DateTime>(type: "datetime", nullable: true),
                    DataInserimento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RigaOrdineFornitore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RigaOrdineFornitore_Articolo",
                        column: x => x.Id,
                        principalTable: "Articolo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RigaOrdineFornitore_OrdineFornitore",
                        column: x => x.Id,
                        principalTable: "OrdineFornitore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RigaOrdineCliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "(newid())"),
                    IdOrdine = table.Column<Guid>(nullable: false),
                    IdArticolo = table.Column<Guid>(nullable: false),
                    Spedito = table.Column<bool>(nullable: false),
                    XXS = table.Column<int>(nullable: false),
                    XS = table.Column<int>(nullable: false),
                    S = table.Column<int>(nullable: false),
                    M = table.Column<int>(nullable: false),
                    L = table.Column<int>(nullable: false),
                    XL = table.Column<int>(nullable: false),
                    XXL = table.Column<int>(nullable: false),
                    XXXL = table.Column<int>(nullable: false),
                    UtenteModifica = table.Column<string>(unicode: false, maxLength: 128, nullable: true),
                    UtenteInserimento = table.Column<string>(unicode: false, maxLength: 128, nullable: false),
                    DataModifica = table.Column<DateTime>(type: "datetime", nullable: true),
                    DataInserimento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RigaOrdineCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RigaOrdineCliente_Articolo1",
                        column: x => x.Id,
                        principalTable: "Articolo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RigaOrdineCliente_OrdineCliente",
                        column: x => x.Id,
                        principalTable: "OrdineCliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articolo_IdCollezione",
                table: "Articolo",
                column: "IdCollezione");

            migrationBuilder.CreateIndex(
                name: "IX_Articolo_IdFornitore",
                table: "Articolo",
                column: "IdFornitore");

            migrationBuilder.CreateIndex(
                name: "IX_Articolo_IdTipo",
                table: "Articolo",
                column: "IdTipo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdineCliente_IdTipoPagamento",
                table: "OrdineCliente",
                column: "IdTipoPagamento");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RigaOrdineCliente");

            migrationBuilder.DropTable(
                name: "RigaOrdineFornitore");

            migrationBuilder.DropTable(
                name: "OrdineCliente");

            migrationBuilder.DropTable(
                name: "Articolo");

            migrationBuilder.DropTable(
                name: "OrdineFornitore");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "TipoPagamento");

            migrationBuilder.DropTable(
                name: "Collezione");

            migrationBuilder.DropTable(
                name: "Fornitore");

            migrationBuilder.DropTable(
                name: "Tipo");
        }
    }
}
