using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class ViewOrdineClienteRiepilogoBreveViewModel
    {
        public Guid Id { get; set; }
        public string Cliente { get; set; }
        public string Agente { get; set; }
        [DisplayName("Data ordine")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataInserimento { get; set; }
        [DisplayName("Data consegna")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataConsegna { get; set; }
        [DisplayName("Pagamento")]
        public string TipoPagamento { get; set; }
        public string Note { get; set; }
        [DisplayName("# Ordine")]
        public string RandomNumber { get; set; }
    }
}
