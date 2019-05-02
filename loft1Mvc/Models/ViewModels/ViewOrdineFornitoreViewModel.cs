using System;
using System.ComponentModel;

namespace StockManagement.Models.ViewModels
{
    public class ViewOrdineFornitoreViewModel
    {
        public Guid Id { get; set; }
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
        [DisplayName("T.U.")]
        public int TagliaUnica { get; set; }
        [DisplayName("€ Acquisto")]
        public double PrezzoAcquisto { get; set; }
        [DisplayName("€ Vendita")]
        public double PrezzoVendita { get; set; }
        public DateTime DataInserimento { get; set; }
    }
}
