using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class Articolo
    {
        public Guid Id { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public Guid IdFornitore { get; set; }
        public string Colore { get; set; }
        public bool Xxs { get; set; }
        public bool Xs { get; set; }
        public bool S { get; set; }
        public bool M { get; set; }
        public bool L { get; set; }
        public bool Xl { get; set; }
        public bool Xxl { get; set; }
        public bool TagliaUnica { get; set; }
        public DateTime TrancheConsegna { get; set; }
        public string Genere { get; set; }
        public Guid IdTipo { get; set; }
        public bool Annullato { get; set; }
        public double PrezzoAcquisto { get; set; }
        public double PrezzoVendita { get; set; }
        public string Foto { get; set; }
        public string Video { get; set; }
        public Guid IdCollezione { get; set; }
        public DateTime DataInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public string UtenteModifica { get; set; }
        public bool Xxxl { get; set; }

        public virtual Collezione IdCollezioneNavigation { get; set; }
        public virtual Fornitore IdFornitoreNavigation { get; set; }
        public virtual Tipo IdTipoNavigation { get; set; }
        public virtual RigaOrdineCliente RigaOrdineCliente { get; set; }
        public virtual RigaOrdineFornitore RigaOrdineFornitore { get; set; }
    }
}
