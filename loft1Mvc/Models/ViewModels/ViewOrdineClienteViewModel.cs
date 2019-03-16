using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{

    public class ViewOrdineClienteViewModel
    {
        public Guid Id { get; set; }
        public Guid IdRappresentante { get; set; }
        public Guid IdCliente { get; set; }

        [DisplayName("Nome cliente")]
        public string NomeCliente { get; set; }

        [DisplayName("Indirizzo cliente")]
        public string IndirizzoCliente { get; set; }

        [DisplayName("Email cliente")]
        public string EmailCliente { get; set; }

        [DisplayName("Data consegna")]
        public DateTime DataConsegna { get; set; }

        public Guid? IdTipoPagamento { get; set; }
        public string Note { get; set; }
        public bool Completato { get; set; }
        public bool Pagato { get; set; }
        public DateTime DataInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public string UtenteModifica { get; set; }

        public double SommaPrezzo { get; set; }

    }
}
