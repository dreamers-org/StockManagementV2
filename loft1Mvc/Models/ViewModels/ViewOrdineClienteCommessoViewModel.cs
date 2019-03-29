using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class ViewOrdineClienteCommessoViewModel
    {
        public Guid Id { get;}
        public string Cliente { get;}
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataConsegna { get; }
        public string TipoPagamento { get;}
        public string Note { get;}
        public bool Spedito { get; }
        public bool Pagato { get;}
        public bool SpeditoInParte { get;}
        public bool Letto { get; }
        public bool Stampato { get; }
    }
}
