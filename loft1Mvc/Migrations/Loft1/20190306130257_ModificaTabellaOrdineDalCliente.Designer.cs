﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockManagement.Models;

namespace loft1Mvc.Migrations.Loft1
{
    [DbContext(typeof(Loft1Context))]
    [Migration("20190306130257_ModificaTabellaOrdineDalCliente")]
    partial class ModificaTabellaOrdineDalCliente
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StockManagement.Models.Articolo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Annullato");

                    b.Property<int>("Codice")
                        .HasMaxLength(50);

                    b.Property<string>("Colore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Descrizione")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Fornitore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<double>("PrezzoAcquisto");

                    b.Property<double>("PrezzoVendita");

                    b.Property<string>("TipoProdotto");

                    b.Property<string>("TrancheConsegna")
                        .IsRequired();

                    b.Property<string>("attr1");

                    b.Property<string>("attr2");

                    b.HasKey("Id");

                    b.ToTable("Articoli");
                });

            modelBuilder.Entity("StockManagement.Models.ArticoloAnnullato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Codice");

                    b.Property<string>("Colore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("ArticoliAnnullati");
                });

            modelBuilder.Entity("StockManagement.Models.OrdineAlFornitore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Codice");

                    b.Property<string>("Colore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Descrizione")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Fornitore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("IdOrdine");

                    b.Property<int?>("L")
                        .HasColumnName("l");

                    b.Property<int?>("M")
                        .HasColumnName("m");

                    b.Property<double>("PrezzoAcquisto");

                    b.Property<double>("PrezzoVendita");

                    b.Property<int?>("S")
                        .HasColumnName("s");

                    b.Property<int?>("Xl")
                        .HasColumnName("xl");

                    b.Property<int?>("Xs")
                        .HasColumnName("xs");

                    b.Property<int?>("Xxl")
                        .HasColumnName("xxl");

                    b.Property<int?>("Xxs")
                        .HasColumnName("xxs");

                    b.Property<int?>("Xxxl")
                        .HasColumnName("xxxl");

                    b.Property<int?>("Xxxs")
                        .HasColumnName("xxxs");

                    b.Property<int?>("Xxxxl")
                        .HasColumnName("xxxxl");

                    b.Property<string>("attr1");

                    b.Property<string>("attr2");

                    b.HasKey("Id");

                    b.ToTable("OrdiniAiFornitori");
                });

            modelBuilder.Entity("StockManagement.Models.OrdineDalCliente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Cliente")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Codice");

                    b.Property<string>("Colore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("DataConsegna")
                        .HasColumnType("date");

                    b.Property<DateTime>("DataOrdine")
                        .HasColumnType("date");

                    b.Property<string>("Descrizione")
                        .IsRequired();

                    b.Property<string>("IdOrdine");

                    b.Property<string>("Indirizzo")
                        .HasMaxLength(50);

                    b.Property<int?>("L")
                        .HasColumnName("l");

                    b.Property<int?>("M")
                        .HasColumnName("m");

                    b.Property<string>("Pagamento")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Rappresentante")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("S")
                        .HasColumnName("s");

                    b.Property<int?>("Xl")
                        .HasColumnName("xl");

                    b.Property<int?>("Xs")
                        .HasColumnName("xs");

                    b.Property<int?>("Xxl")
                        .HasColumnName("xxl");

                    b.Property<int?>("Xxs")
                        .HasColumnName("xxs");

                    b.Property<int?>("Xxxl")
                        .HasColumnName("xxxl");

                    b.Property<int?>("Xxxs")
                        .HasColumnName("xxxs");

                    b.Property<int?>("Xxxxl")
                        .HasColumnName("xxxxl");

                    b.Property<string>("attr1");

                    b.Property<string>("attr2");

                    b.HasKey("Id");

                    b.ToTable("OrdiniDaiClienti");
                });

            modelBuilder.Entity("StockManagement.Models.PackingList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Codice");

                    b.Property<string>("Fornitore")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("L")
                        .HasColumnName("l");

                    b.Property<int?>("M")
                        .HasColumnName("m");

                    b.Property<int?>("S")
                        .HasColumnName("s");

                    b.Property<string>("Variante")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("Xl")
                        .HasColumnName("xl");

                    b.Property<int?>("Xs")
                        .HasColumnName("xs");

                    b.Property<int?>("Xxl")
                        .HasColumnName("xxl");

                    b.Property<int?>("Xxs")
                        .HasColumnName("xxs");

                    b.Property<int?>("Xxxl")
                        .HasColumnName("xxxl");

                    b.Property<int?>("Xxxs")
                        .HasColumnName("xxxs");

                    b.Property<int?>("Xxxxl")
                        .HasColumnName("xxxxl");

                    b.Property<string>("attr1");

                    b.Property<string>("attr2");

                    b.HasKey("Id");

                    b.ToTable("PackingList");
                });

            modelBuilder.Entity("StockManagement.Models.TipoProdotto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("TipoProdotto");
                });

            modelBuilder.Entity("StockManagement.Models.TrancheConsegna", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DataConsegna")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("TrancheConsegna");
                });
#pragma warning restore 612, 618
        }
    }
}
