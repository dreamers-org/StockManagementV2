using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class OrdineFornitore
    {
        public Guid Id { get; set; }
        public DateTime DataInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public string UtenteModifica { get; set; }

        public virtual RigaOrdineFornitore RigaOrdineFornitore { get; set; }
    }
}
