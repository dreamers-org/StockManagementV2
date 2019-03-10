using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class OrdineCliente
    {
        public Guid Id { get; set; }
        public Guid IdRappresentante { get; set; }
        public Guid IdCliente { get; set; }
        public DateTime DataConsegna { get; set; }
        public Guid IdPagamento { get; set; }
        public string Note { get; set; }
        public bool Completato { get; set; }
        public bool Pagato { get; set; }
        public DateTime DataInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public string UtenteModifica { get; set; }

        public virtual Cliente IdNavigation { get; set; }
        public virtual TipoPagamento IdPagamentoNavigation { get; set; }
        public virtual RigaOrdineCliente RigaOrdineCliente { get; set; }
    }
}
