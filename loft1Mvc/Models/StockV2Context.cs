using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StockManagement.Models.ViewModels;

namespace StockManagement.Models
{
    public partial class StockV2Context : DbContext
    {
        public StockV2Context()
        {
        }

        public StockV2Context(DbContextOptions<StockV2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Articolo> Articolo { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Collezione> Collezione { get; set; }
        public virtual DbSet<Fornitore> Fornitore { get; set; }
        public virtual DbSet<OrdineCliente> OrdineCliente { get; set; }
        public virtual DbSet<OrdineFornitore> OrdineFornitore { get; set; }
        public virtual DbSet<RigaOrdineCliente> RigaOrdineCliente { get; set; }
        public virtual DbSet<RigaOrdineFornitore> RigaOrdineFornitore { get; set; }
        public virtual DbSet<Tipo> Tipo { get; set; }
        public virtual DbSet<PackingList> PackingList { get; set; }
        public virtual DbSet<TipoPagamento> TipoPagamento { get; set; }
        public virtual DbQuery<ViewOrdineClienteViewModel> ViewOrdineCliente { get; set; }
        public virtual DbQuery<ViewPackingListViewModel> ViewPackingList { get; set; }

        public virtual DbQuery<ViewRigaOrdineClienteViewModel> ViewRigaOrdineCliente { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Data Source=loft1mvc.database.windows.net;Initial Catalog=Stock;User ID=luca.bellavia.dev;Password=Pallone27@@;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<OrdineClienteViewViewModel>();

            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Articolo>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Codice)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Colore)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.DataInserimento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataModifica).HasColumnType("datetime");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Foto)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.Genere)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.TrancheConsegna).HasColumnType("datetime");

                entity.Property(e => e.UtenteInserimento)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UtenteModifica)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Video)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.Xl).HasColumnName("XL");

                entity.Property(e => e.Xs).HasColumnName("XS");

                entity.Property(e => e.Xxl).HasColumnName("XXL");

                entity.Property(e => e.Xxs).HasColumnName("XXS");

                entity.Property(e => e.Xxxl).HasColumnName("XXXL");

                entity.HasOne(d => d.IdCollezioneNavigation)
                    .WithMany(p => p.Articolo)
                    .HasForeignKey(d => d.IdCollezione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Articolo_Collezione1");

                entity.HasOne(d => d.IdFornitoreNavigation)
                    .WithMany(p => p.Articolo)
                    .HasForeignKey(d => d.IdFornitore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Articolo_Fornitore1");

                entity.HasOne(d => d.IdTipoNavigation)
                    .WithMany(p => p.Articolo)
                    .HasForeignKey(d => d.IdTipo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Articolo_Tipo1");


            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Attivo)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Indirizzo)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Collezione>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Fornitore>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrdineCliente>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DataConsegna).HasColumnType("datetime");

                entity.Property(e => e.DataInserimento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataModifica).HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.UtenteInserimento)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UtenteModifica)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.OrdineCliente)
                    .HasForeignKey<OrdineCliente>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrdineCliente_Cliente");

                entity.HasOne(d => d.IdPagamentoNavigation)
                    .WithMany(p => p.OrdineCliente)
                    .HasForeignKey(d => d.IdTipoPagamento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrdineCliente_TipoPagamento");

                entity.Property(e => e.Spedito)
                 .HasDefaultValue(false);

                entity.Property(e => e.SpeditoInParte)
                 .HasDefaultValue(false);

                entity.Property(e => e.Letto)
                 .HasDefaultValue(false);

                entity.Property(e => e.Stampato)
                 .HasDefaultValue(false);
            });

            modelBuilder.Entity<OrdineFornitore>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DataInserimento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataModifica).HasColumnType("datetime");

                entity.Property(e => e.UtenteInserimento)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UtenteModifica)
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RigaOrdineCliente>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DataInserimento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataModifica).HasColumnType("datetime");

                entity.Property(e => e.UtenteInserimento)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UtenteModifica)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Xl).HasColumnName("XL");

                entity.Property(e => e.Xs).HasColumnName("XS");

                entity.Property(e => e.Xxl).HasColumnName("XXL");

                entity.Property(e => e.Xxs).HasColumnName("XXS");

                entity.Property(e => e.Xxxl).HasColumnName("XXXL");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.RigaOrdineCliente)
                    .HasForeignKey<RigaOrdineCliente>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RigaOrdineCliente_Articolo1");

                entity.HasOne(d => d.Id1)
                    .WithOne(p => p.RigaOrdineCliente)
                    .HasForeignKey<RigaOrdineCliente>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RigaOrdineCliente_OrdineCliente");
            });

            modelBuilder.Entity<RigaOrdineFornitore>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DataInserimento)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataModifica).HasColumnType("datetime");

                entity.Property(e => e.Larrivato).HasColumnName("LArrivato");

                entity.Property(e => e.Marrivato).HasColumnName("MArrivato");

                entity.Property(e => e.Sarrivato).HasColumnName("SArrivato");

                entity.Property(e => e.UtenteInserimento)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UtenteModifica)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Xl).HasColumnName("XL");

                entity.Property(e => e.Xlarrivato).HasColumnName("XLArrivato");

                entity.Property(e => e.Xs).HasColumnName("XS");

                entity.Property(e => e.Xsarrivato).HasColumnName("XSArrivato");

                entity.Property(e => e.Xxl).HasColumnName("XXL");

                entity.Property(e => e.Xxlarrivato).HasColumnName("XXLArrivato");

                entity.Property(e => e.Xxs).HasColumnName("XXS");

                entity.Property(e => e.Xxsarrivato).HasColumnName("XXSArrivato");

                entity.Property(e => e.Xxxl).HasColumnName("XXXL");

                entity.Property(e => e.Xxxlarrivato).HasColumnName("XXXLArrivato");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.RigaOrdineFornitore)
                    .HasForeignKey<RigaOrdineFornitore>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RigaOrdineFornitore_Articolo");

                entity.HasOne(d => d.Id1)
                    .WithOne(p => p.RigaOrdineFornitore)
                    .HasForeignKey<RigaOrdineFornitore>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RigaOrdineFornitore_OrdineFornitore");
            });

            modelBuilder.Entity<Tipo>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoPagamento>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PackingList>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });
        }
    }
}
