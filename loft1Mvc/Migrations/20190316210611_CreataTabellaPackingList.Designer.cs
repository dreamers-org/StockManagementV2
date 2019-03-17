﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockManagement.Models;

namespace StockManagement.Migrations
{
    [DbContext(typeof(StockV2Context))]
    [Migration("20190316210611_CreataTabellaPackingList")]
    partial class CreataTabellaPackingList
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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<bool>("Annullato");

                    b.Property<string>("Codice")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(false);

                    b.Property<string>("Colore")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(false);

                    b.Property<DateTime>("DataInserimento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DataModifica")
                        .HasColumnType("datetime");

                    b.Property<string>("Descrizione")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(false);

                    b.Property<string>("Foto")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .IsUnicode(false);

                    b.Property<string>("Genere")
                        .IsRequired()
                        .HasMaxLength(16)
                        .IsUnicode(false);

                    b.Property<Guid>("IdCollezione");

                    b.Property<Guid>("IdFornitore");

                    b.Property<Guid>("IdTipo");

                    b.Property<bool>("L");

                    b.Property<bool>("M");

                    b.Property<double>("PrezzoAcquisto");

                    b.Property<double>("PrezzoVendita");

                    b.Property<bool>("S");

                    b.Property<bool>("TagliaUnica");

                    b.Property<DateTime>("TrancheConsegna")
                        .HasColumnType("datetime");

                    b.Property<string>("UtenteInserimento")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("UtenteModifica")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("Video")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .IsUnicode(false);

                    b.Property<bool>("Xl")
                        .HasColumnName("XL");

                    b.Property<bool>("Xs")
                        .HasColumnName("XS");

                    b.Property<bool>("Xxl")
                        .HasColumnName("XXL");

                    b.Property<bool>("Xxs")
                        .HasColumnName("XXS");

                    b.Property<bool>("Xxxl")
                        .HasColumnName("XXXL");

                    b.HasKey("Id");

                    b.HasIndex("IdCollezione");

                    b.HasIndex("IdFornitore");

                    b.HasIndex("IdTipo");

                    b.ToTable("Articolo");
                });

            modelBuilder.Entity("StockManagement.Models.Cliente", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<bool?>("Attivo")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((1))");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("Indirizzo")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Cliente");
                });

            modelBuilder.Entity("StockManagement.Models.Collezione", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Collezione");
                });

            modelBuilder.Entity("StockManagement.Models.Fornitore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Fornitore");
                });

            modelBuilder.Entity("StockManagement.Models.OrdineCliente", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<bool>("Completato");

                    b.Property<DateTime>("DataConsegna")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DataInserimento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DataModifica")
                        .HasColumnType("datetime");

                    b.Property<Guid>("IdCliente");

                    b.Property<Guid>("IdRappresentante");

                    b.Property<Guid?>("IdTipoPagamento");

                    b.Property<string>("Note")
                        .HasMaxLength(8000)
                        .IsUnicode(false);

                    b.Property<bool>("Pagato");

                    b.Property<string>("UtenteInserimento")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("UtenteModifica")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("IdTipoPagamento");

                    b.ToTable("OrdineCliente");
                });

            modelBuilder.Entity("StockManagement.Models.OrdineFornitore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("DataInserimento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DataModifica")
                        .HasColumnType("datetime");

                    b.Property<string>("UtenteInserimento")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("UtenteModifica")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("OrdineFornitore");
                });

            modelBuilder.Entity("StockManagement.Models.PackingList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("DataInserimento");

                    b.Property<Guid?>("IdArticoloId");

                    b.Property<int>("L");

                    b.Property<int>("M");

                    b.Property<int>("S");

                    b.Property<int>("TagliaUnica");

                    b.Property<string>("UtenteInserimento");

                    b.Property<int>("Xl");

                    b.Property<int>("Xs");

                    b.Property<int>("Xxl");

                    b.Property<int>("Xxs");

                    b.Property<int>("Xxxl");

                    b.HasKey("Id");

                    b.HasIndex("IdArticoloId");

                    b.ToTable("PackingList");
                });

            modelBuilder.Entity("StockManagement.Models.RigaOrdineCliente", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("DataInserimento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DataModifica")
                        .HasColumnType("datetime");

                    b.Property<Guid>("IdArticolo");

                    b.Property<Guid>("IdOrdine");

                    b.Property<int>("L");

                    b.Property<int>("M");

                    b.Property<int>("S");

                    b.Property<bool>("Spedito");

                    b.Property<string>("UtenteInserimento")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("UtenteModifica")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<int>("Xl")
                        .HasColumnName("XL");

                    b.Property<int>("Xs")
                        .HasColumnName("XS");

                    b.Property<int>("Xxl")
                        .HasColumnName("XXL");

                    b.Property<int>("Xxs")
                        .HasColumnName("XXS");

                    b.Property<int>("Xxxl")
                        .HasColumnName("XXXL");

                    b.HasKey("Id");

                    b.ToTable("RigaOrdineCliente");
                });

            modelBuilder.Entity("StockManagement.Models.RigaOrdineFornitore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("DataInserimento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("DataModifica")
                        .HasColumnType("datetime");

                    b.Property<Guid>("IdArticolo");

                    b.Property<Guid>("IdOrdine");

                    b.Property<int>("L");

                    b.Property<int>("Larrivato")
                        .HasColumnName("LArrivato");

                    b.Property<int>("M");

                    b.Property<int>("Marrivato")
                        .HasColumnName("MArrivato");

                    b.Property<int>("S");

                    b.Property<int>("Sarrivato")
                        .HasColumnName("SArrivato");

                    b.Property<string>("UtenteInserimento")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<string>("UtenteModifica")
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.Property<int>("Xl")
                        .HasColumnName("XL");

                    b.Property<int>("Xlarrivato")
                        .HasColumnName("XLArrivato");

                    b.Property<int>("Xs")
                        .HasColumnName("XS");

                    b.Property<int>("Xsarrivato")
                        .HasColumnName("XSArrivato");

                    b.Property<int>("Xxl")
                        .HasColumnName("XXL");

                    b.Property<int>("Xxlarrivato")
                        .HasColumnName("XXLArrivato");

                    b.Property<int>("Xxs")
                        .HasColumnName("XXS");

                    b.Property<int>("Xxsarrivato")
                        .HasColumnName("XXSArrivato");

                    b.Property<int>("Xxxl")
                        .HasColumnName("XXXL");

                    b.Property<int>("Xxxlarrivato")
                        .HasColumnName("XXXLArrivato");

                    b.HasKey("Id");

                    b.ToTable("RigaOrdineFornitore");
                });

            modelBuilder.Entity("StockManagement.Models.Tipo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Tipo");
                });

            modelBuilder.Entity("StockManagement.Models.TipoPagamento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(64)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("TipoPagamento");
                });

            modelBuilder.Entity("StockManagement.Models.Articolo", b =>
                {
                    b.HasOne("StockManagement.Models.Collezione", "IdCollezioneNavigation")
                        .WithMany("Articolo")
                        .HasForeignKey("IdCollezione")
                        .HasConstraintName("FK_Articolo_Collezione1");

                    b.HasOne("StockManagement.Models.Fornitore", "IdFornitoreNavigation")
                        .WithMany("Articolo")
                        .HasForeignKey("IdFornitore")
                        .HasConstraintName("FK_Articolo_Fornitore1");

                    b.HasOne("StockManagement.Models.Tipo", "IdTipoNavigation")
                        .WithMany("Articolo")
                        .HasForeignKey("IdTipo")
                        .HasConstraintName("FK_Articolo_Tipo1");
                });

            modelBuilder.Entity("StockManagement.Models.OrdineCliente", b =>
                {
                    b.HasOne("StockManagement.Models.Cliente", "IdNavigation")
                        .WithOne("OrdineCliente")
                        .HasForeignKey("StockManagement.Models.OrdineCliente", "Id")
                        .HasConstraintName("FK_OrdineCliente_Cliente");

                    b.HasOne("StockManagement.Models.TipoPagamento", "IdPagamentoNavigation")
                        .WithMany("OrdineCliente")
                        .HasForeignKey("IdTipoPagamento")
                        .HasConstraintName("FK_OrdineCliente_TipoPagamento");
                });

            modelBuilder.Entity("StockManagement.Models.PackingList", b =>
                {
                    b.HasOne("StockManagement.Models.Articolo", "IdArticolo")
                        .WithMany()
                        .HasForeignKey("IdArticoloId");
                });

            modelBuilder.Entity("StockManagement.Models.RigaOrdineCliente", b =>
                {
                    b.HasOne("StockManagement.Models.Articolo", "IdNavigation")
                        .WithOne("RigaOrdineCliente")
                        .HasForeignKey("StockManagement.Models.RigaOrdineCliente", "Id")
                        .HasConstraintName("FK_RigaOrdineCliente_Articolo1");

                    b.HasOne("StockManagement.Models.OrdineCliente", "Id1")
                        .WithOne("RigaOrdineCliente")
                        .HasForeignKey("StockManagement.Models.RigaOrdineCliente", "Id")
                        .HasConstraintName("FK_RigaOrdineCliente_OrdineCliente");
                });

            modelBuilder.Entity("StockManagement.Models.RigaOrdineFornitore", b =>
                {
                    b.HasOne("StockManagement.Models.Articolo", "IdNavigation")
                        .WithOne("RigaOrdineFornitore")
                        .HasForeignKey("StockManagement.Models.RigaOrdineFornitore", "Id")
                        .HasConstraintName("FK_RigaOrdineFornitore_Articolo");

                    b.HasOne("StockManagement.Models.OrdineFornitore", "Id1")
                        .WithOne("RigaOrdineFornitore")
                        .HasForeignKey("StockManagement.Models.RigaOrdineFornitore", "Id")
                        .HasConstraintName("FK_RigaOrdineFornitore_OrdineFornitore");
                });
#pragma warning restore 612, 618
        }
    }
}
