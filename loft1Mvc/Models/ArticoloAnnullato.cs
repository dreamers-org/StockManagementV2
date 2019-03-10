using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public partial class ArticoloAnnullato
    {
        public int Id { get; set; }
		[DisplayName("Codice")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Codice { get; set; }
		[DisplayName("Colore")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Colore { get; set; }
    }
}
