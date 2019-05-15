using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class ViewRigaOrdineClienteCommessoViewModel
    {
        public string Fornitore { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public string Colore { get; set; }
        [DisplayName("2XS/40")]
        public int Xxs { get; set; }
        [DisplayName("XS/42")]
        public int Xs { get; set; }
        [DisplayName("S/44")]
        public int S { get; set; }
        [DisplayName("M/46")]
        public int M { get; set; }
        [DisplayName("L/48")]
        public int L { get; set; }
        [DisplayName("XL/50")]
        public int Xl { get; set; }
        [DisplayName("2XL/52")]
        public int Xxl { get; set; }
        [DisplayName("3XL/54")]
        public int Xxxl { get; set; }
        [DisplayName("4XL/56")]
        public int Xxxxl { get; set; }
        [DisplayName("T.U.")]
        public int TagliaUnica { get; set; }
        public Guid IdRigaOrdine { get; set; }
        public Guid IdOrdine { get; set; }

        [DisplayName("€/articolo")]
        public double PrezzoArticolo { get; set; }

        [DisplayName("N. articoli")]
        public int TotalePezzi { get; set; }

        [DisplayName("€/riga")]
        public double TotaleRiga { get; set; }
    }
}
