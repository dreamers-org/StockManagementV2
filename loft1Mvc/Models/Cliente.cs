using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class Cliente
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public string Email { get; set; }
        public bool? Attivo { get; set; }

        public virtual OrdineCliente OrdineCliente { get; set; }
    }
}
