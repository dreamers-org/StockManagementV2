using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models.ViewModels
{
    public class ViewOrdineClienteCommessoViewModel
    {
        public Guid Id { get; set; }
        public string Cliente { get; set; }
        public string AgenziaRappresentanza { get; set; }
        [DisplayName("Consegna")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataConsegna { get; set; }
        public DateTime DataInserimento { get; set; }
        [DisplayName("Pagamento")]
        public string TipoPagamento { get; set; }
        public string Note { get; set; }
        public bool Completato { get; set; }
        public bool Pagato { get; set;}
        public bool Spedito { get; set; }
        [DisplayName("Spedito in parte")]
        public bool SpeditoInParte { get; set; }
        public bool Letto { get; set; }
        public bool Stampato { get; set; }
        [DisplayName("# Ordine")]
        public string RandomNumber { get; set; }
        public long CondizioniAccettate { get; set; }

    }
}
