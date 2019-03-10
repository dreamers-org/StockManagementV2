using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class RigaOrdineFornitore
    {
        public Guid Id { get; set; }
        public Guid IdOrdine { get; set; }
        public Guid IdArticolo { get; set; }
        public int Xxsarrivato { get; set; }
        public int Xxs { get; set; }
        public int Xs { get; set; }
        public int Xsarrivato { get; set; }
        public int S { get; set; }
        public int Sarrivato { get; set; }
        public int M { get; set; }
        public int Marrivato { get; set; }
        public int L { get; set; }
        public int Larrivato { get; set; }
        public int Xl { get; set; }
        public int Xlarrivato { get; set; }
        public int Xxl { get; set; }
        public int Xxlarrivato { get; set; }
        public int Xxxl { get; set; }
        public int Xxxlarrivato { get; set; }
        public string UtenteModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public DateTime DataInserimento { get; set; }

        public virtual OrdineFornitore Id1 { get; set; }
        public virtual Articolo IdNavigation { get; set; }
    }
}
