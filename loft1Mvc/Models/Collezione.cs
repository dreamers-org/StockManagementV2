using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class Collezione
    {
        public Collezione()
        {
            Articolo = new HashSet<Articolo>();
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }

        public virtual ICollection<Articolo> Articolo { get; set; }
    }
}
