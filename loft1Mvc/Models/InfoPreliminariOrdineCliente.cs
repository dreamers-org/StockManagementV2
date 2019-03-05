using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace loft1Mvc.Models
{
	public class InfoPreliminariOrdineCliente
	{
		[Required(ErrorMessage = "Obbligatorio.")]
		[DisplayName("Cliente")]
		public string Cliente { get; set; }
		[Required(ErrorMessage = "Obbligatorio.")]
		[DisplayName("Indirizzo")]
		public string Indirizzo { get; set; }
		[Required(ErrorMessage = "Obbligatorio.")]
		[DisplayName("Consegna richiesta")]
		[DataType(DataType.Date)]
		public DateTime DataConsegna { get; set; }
	}
}
