using Microsoft.EntityFrameworkCore;

namespace loft1Mvc.Models
{
	public partial class Loft1Context : DbContext
	{
		public Loft1Context()
		{
		}

		public Loft1Context(DbContextOptions<Loft1Context> options)
			: base(options)
		{
		}

		public virtual DbSet<Articolo> Articoli { get; set; }
		public virtual DbSet<ArticoloAnnullato> ArticoliAnnullati { get; set; }
		public virtual DbSet<TipoProdotto> TipoProdotto { get; set; }
		public virtual DbSet<TrancheConsegna> TrancheConsegna { get; set; }
		public virtual DbSet<OrdineAlFornitore> OrdiniAiFornitori { get; set; }
		public virtual DbSet<OrdineDalCliente> OrdiniDaiClienti { get; set; }
		public virtual DbSet<PackingList> PackingList { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Data Source=loft1mvc.database.windows.net;Initial Catalog=Stock;User ID=luca.bellavia.dev;Password=Pallone27@@;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

			modelBuilder.Entity<TipoProdotto>(entity =>
			{
				entity.Property(e => e.Tipo)
					.IsRequired()
					.HasMaxLength(50);
			});

			modelBuilder.Entity<TrancheConsegna>(entity =>
			{
				entity.Property(e => e.DataConsegna).HasColumnType("date");
			});

			modelBuilder.Entity<Articolo>(entity =>
			{
				entity.Property(e => e.Codice)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Colore)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Descrizione)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Fornitore)
					.IsRequired()
					.HasMaxLength(50);
			});

			modelBuilder.Entity<ArticoloAnnullato>(entity =>
			{
				entity.Property(e => e.Colore)
					.IsRequired()
					.HasMaxLength(50);
			});

			modelBuilder.Entity<OrdineAlFornitore>(entity =>
			{
				entity.Property(e => e.Colore)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Descrizione)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Fornitore)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.L).HasColumnName("l");

				entity.Property(e => e.M).HasColumnName("m");

				entity.Property(e => e.S).HasColumnName("s");

				entity.Property(e => e.Xl).HasColumnName("xl");

				entity.Property(e => e.Xs).HasColumnName("xs");

				entity.Property(e => e.Xxl).HasColumnName("xxl");

				entity.Property(e => e.Xxs).HasColumnName("xxs");

				entity.Property(e => e.Xxxl).HasColumnName("xxxl");

				entity.Property(e => e.Xxxs).HasColumnName("xxxs");

				entity.Property(e => e.Xxxxl).HasColumnName("xxxxl");
			});

			modelBuilder.Entity<OrdineDalCliente>(entity =>
			{
				entity.Property(e => e.Cliente)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Colore)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.DataConsegna).HasColumnType("date");

				entity.Property(e => e.DataOrdine).HasColumnType("date");

				entity.Property(e => e.Indirizzo).HasMaxLength(50);

				entity.Property(e => e.L).HasColumnName("l");

				entity.Property(e => e.M).HasColumnName("m");

				entity.Property(e => e.Pagamento)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Rappresentante)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.S).HasColumnName("s");

				entity.Property(e => e.Xl).HasColumnName("xl");

				entity.Property(e => e.Xs).HasColumnName("xs");

				entity.Property(e => e.Xxl).HasColumnName("xxl");

				entity.Property(e => e.Xxs).HasColumnName("xxs");

				entity.Property(e => e.Xxxl).HasColumnName("xxxl");

				entity.Property(e => e.Xxxs).HasColumnName("xxxs");

				entity.Property(e => e.Xxxxl).HasColumnName("xxxxl");
			});

			modelBuilder.Entity<PackingList>(entity =>
			{
				entity.Property(e => e.Fornitore)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.L).HasColumnName("l");

				entity.Property(e => e.M).HasColumnName("m");

				entity.Property(e => e.S).HasColumnName("s");

				entity.Property(e => e.Variante)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Xl).HasColumnName("xl");

				entity.Property(e => e.Xs).HasColumnName("xs");

				entity.Property(e => e.Xxl).HasColumnName("xxl");

				entity.Property(e => e.Xxs).HasColumnName("xxs");

				entity.Property(e => e.Xxxl).HasColumnName("xxxl");

				entity.Property(e => e.Xxxs).HasColumnName("xxxs");

				entity.Property(e => e.Xxxxl).HasColumnName("xxxxl");
			});

		}
	}
}
