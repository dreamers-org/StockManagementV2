using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
	//Rappresenta la classe Articolo senza le colonne che verranno compilate e inserite lato codice a runtime
	public class ArticoloViewModel
    {
		public int Id { get; set; }
		[DisplayName("Fornitore")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Fornitore { get; set; }
		[DisplayName("Tipo")]
		[RegularExpression(@"^(?!Tipo Prodotto$).*$", ErrorMessage = "Inserire un tipo valido.")]
		public string TipoProdotto { get; set; }
		[DisplayName("Genere")]
		[RegularExpression(@"^(?!Tipo Prodotto$).*$", ErrorMessage = "Inserire un tipo valido.")]
		public string Genere { get; set; }
		[DisplayName("Cod.")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Codice { get; set; }
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
		[DisplayName("Annullato")]
		public bool Annullato { get; set; }
		[DisplayName("3XS")]
		[DefaultValue(false)]
		public bool Xxxs { get; set; }
		[DisplayName("XXS")]
		[DefaultValue(false)]
		public bool Xxs { get; set; }
		[DisplayName("XS")]
		[DefaultValue(false)]
		public bool Xs { get; set; }
		[DisplayName("S")]
		[DefaultValue(false)]
		public bool S { get; set; }
		[DisplayName("M")]
		[DefaultValue(false)]
		public bool M { get; set; }
		[DisplayName("L")]
		[DefaultValue(false)]
		public bool L { get; set; }
		[DisplayName("XL")]
		[DefaultValue(false)]
		public bool Xl { get; set; }
		[DisplayName("XXL")]
		[DefaultValue(false)]
		public bool Xxl { get; set; }
		[DisplayName("3XL")]
		[DefaultValue(false)]
		public bool Xxxl { get; set; }
		[DisplayName("4XL")]
		[DefaultValue(false)]
		public bool Xxxxl { get; set; }
	}
}
