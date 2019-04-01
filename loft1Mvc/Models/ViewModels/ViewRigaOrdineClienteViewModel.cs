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
        public int Xxs { get; set; }
        public int Xs { get; set; }
        public int S { get; set; }
        public int M { get; set; }
        public int L { get; set; }
        public int Xl { get; set; }
        public int Xxl { get; set; }
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
    }
}
