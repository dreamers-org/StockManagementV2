using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class TipoPagamento
    {
        public TipoPagamento()
        {
            OrdineCliente = new HashSet<OrdineCliente>();
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }

        public virtual ICollection<OrdineCliente> OrdineCliente { get; set; }
    }
}
