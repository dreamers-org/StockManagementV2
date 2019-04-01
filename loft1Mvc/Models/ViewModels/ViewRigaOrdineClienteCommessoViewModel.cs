using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class ViewRigaOrdineClienteCommessoViewModel
    {
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public string Colore { get; set; }
        public int Xxs { get; set; }
        public int Xs { get; set; }
        public int S { get; set; }
        public int M { get; set; }
        public int L { get; set; }
        public int Xl { get; set; }
        public int Xxl { get; set; }
        public int Xxxl { get; set; }
        //public int? TagliaUnica { get; set; } //TODO: Togliere il fatto che sia nullabile
        public Guid IdRigaOrdine { get; set; }
        public Guid IdOrdine { get; set; }
    }
}
