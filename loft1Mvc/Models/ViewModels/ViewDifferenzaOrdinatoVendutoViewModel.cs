using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models.ViewModels
{
    public class ViewDifferenzaOrdinatoVendutoViewModel
    {
        public string Fornitore { get; set; }

        public string Collezione { get; set; }

        public string Codice { get; set; }

        public string Colore { get; set; }

        public string Descrizione { get; set; }

        public int VXXS { get; set; }

        public int VXS { get; set; }

        public int VS { get; set; }

        public int VM { get; set; }

        public int VL { get; set; }

        public int VXL { get; set; }

        public int VXXL { get; set; }

        public int VXXXL { get; set; }

        public int VXXXXL { get; set; }

        public int VTU { get; set; }

        public int CXXS { get; set; }

        public int CXS { get; set; }

        public int CS { get; set; }

        public int CM { get; set; }

        public int CL { get; set; }

        public int CXL { get; set; }

        public int CXXL { get; set; }

        public int CXXXL { get; set; }

        public int CXXXXL { get; set; }

        public int CTU { get; set; }

        public int DIFFXXS { get; set; }

        public int DIFFXS { get; set; }

        public int DIFFS { get; set; }

        public int DIFFM { get; set; }

        public int DIFFL { get; set; }

        public int DIFFXL { get; set; }

        public int DIFFXXL { get; set; }

        public int DIFFXXXL { get; set; }

        public int DIFFXXXXL { get; set; }

        public int DIFFTU { get; set; }

        public double PrezzoAcquisto { get; set; }
        public double PrezzoVendita { get; set; }
        public double TotPrezzoAcquisto { get; set; }
        public double TotPrezzoVendita { get; set; }
        public double DifferenzaTotPrezzo { get; set; }

    }
}
