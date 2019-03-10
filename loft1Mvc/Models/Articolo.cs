using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
	public class Articolo : ArticoloViewModel
	{
		[DisplayName("Operatore Inserimento")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Operatore { get; set; }
		[DisplayName("Data Inserimento")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string DataInserimento { get; set; }

		public Articolo(ArticoloViewModel art) : base()
		{
		}
	}
}
