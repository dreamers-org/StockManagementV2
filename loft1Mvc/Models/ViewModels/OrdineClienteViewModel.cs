using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class OrdineClienteViewModel:OrdineCliente
    {
		public string NomeCliente { get; set; }
		public string IndirizzoCliente { get; set; }
        public string EmailCliente { get; set; }

        public string CodiceArticolo { get; set; }
        public string ColoreArticolo { get; set; }

        public int Xxs { get; set; }
        public int Xs { get; set; }
        public int S { get; set; }
        public int M { get; set; }
        public int L { get; set; }
        public int Xl { get; set; }
        public int Xxl { get; set; }
        public int Xxxl { get; set; }
    }
}
