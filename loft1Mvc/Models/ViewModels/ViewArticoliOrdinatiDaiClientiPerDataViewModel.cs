using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class ViewArticoliOrdinatiDaiClientiPerDataViewModel
    {
        public string Fornitore { get; set; }
        public string Cliente { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public string Colore { get; set; }
        public int XXS{ get; set; }
        public int XS{ get; set; }
        public int S{ get; set; }
        public int M{ get; set; }
        public int L{ get; set; }
        public int XL{ get; set; }
        public int XXL{ get; set; }
        public int XXXL{ get; set; }
        public int XXXXL{ get; set; }
        public int TagliaUnica { get; set; }
        public DateTime DataConsegna { get; set; }
    }
}
