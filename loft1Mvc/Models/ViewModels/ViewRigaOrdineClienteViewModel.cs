using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{

    public class ViewRigaOrdineClienteViewModel
    {
        public Guid Id { get; set; }
        public Guid IdOrdine { get; set; }
        public Guid IdArticolo { get; set; }
        public bool Spedito { get; set; }
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
        public int TagliaUnica { get; set; }
        public string UtenteModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public DateTime DataInserimento { get; set; }

        [DisplayName("Cod.")]
        public string CodiceArticolo { get; set; }

        [DisplayName("Descr.")]
        public string DescrizioneArticolo { get; set; }

        [DisplayName("Colore")]
        public string Colore { get; set; }

        [DisplayName("€")]
        public double PrezzoVendita { get; set; }


    }
}
