using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace loft1Mvc.Models
{
	public partial class OrdineDalCliente
	{
		public int Id { get; set; }
        public string IdOrdine { get; set; }
		[DisplayName("Cliente")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Cliente { get; set; }


		public string Rappresentante { get; set; }

		[DisplayName("Data ordine")]
		//[Required(ErrorMessage = "Obbligatorio.")]
		[DataType(DataType.Date,ErrorMessage = "Inserire una data obbligatoria.")]
		[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
		public DateTime DataOrdine { get; set; }
		[DisplayName("Consegna")]
		[Required(ErrorMessage = "Obbligatorio.")]
		[DataType(DataType.Date, ErrorMessage = "Inserire una data obbligatoria.")]
		[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
		public DateTime DataConsegna { get; set; }
		[DisplayName("Indirizzo")]
		public string Indirizzo { get; set; }
		

		public string Pagamento { get; set; }
		[DisplayName("Cod.")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public int Codice { get; set; }
		[DisplayName("Descrizione")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Descrizione { get; set; }
		[DisplayName("Colore")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Colore { get; set; }
		[DisplayName("3XS")]
		public int? Xxxs { get; set; }
		[DisplayName("XXS")]
		public int? Xxs { get; set; }
		[DisplayName("XS")]
		public int? Xs { get; set; }
		[DisplayName("S")]
		public int? S { get; set; }
		[DisplayName("M")]
		public int? M { get; set; }
		[DisplayName("L")]
		public int? L { get; set; }
		[DisplayName("XL")]
		public int? Xl { get; set; }
		[DisplayName("XXL")]
		public int? Xxl { get; set; }
		[DisplayName("3XL")]
		public int? Xxxl { get; set; }
		[DisplayName("4XL")]
		public int? Xxxxl { get; set; }
		public string attr1 { get; set; }
		public string attr2 { get; set; }
	}
}
