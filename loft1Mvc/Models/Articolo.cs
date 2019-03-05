using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace loft1Mvc.Models
{
	public partial class Articolo
	{
		public int Id { get; set; }
		[DisplayName("Fornitore")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Fornitore { get; set; }
		[DisplayName("Tipo")]
		[RegularExpression(@"^(?!Tipo Prodotto$).*$", ErrorMessage = "Inserire un tipo valido.")]
		public string TipoProdotto { get; set; }
		[DisplayName("Cod.")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public int Codice { get; set; }
		[DisplayName("Descr.")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Descrizione { get; set; }
		[DisplayName("Colore")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Colore { get; set; }
		[DisplayName("Consegna")]
		[Required(ErrorMessage = "Obbligatorio.")]
		[DataType(DataType.Date)]
		public string TrancheConsegna { get; set; }
		[DisplayName("€ Acquisto")]
		[Required(ErrorMessage = "Obbligatorio.")]
		[RegularExpression(@"^\d+(.\d{1,2})?$", ErrorMessage = "Inserire un prezzo valido.")]
		public double PrezzoAcquisto { get; set; }
		[DisplayName("€ Vendita")]
		[Required(ErrorMessage = "Obbligatorio.")]
		[RegularExpression(@"^\d+(.\d{1,2})?$", ErrorMessage = "Inserire un prezzo valido.")]
		public double PrezzoVendita { get; set; }
		public bool Annullato { get; set; }
		public string attr1 { get; set; }
		public string attr2 { get; set; }
	}
}
