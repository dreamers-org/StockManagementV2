using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class ArticoloAnnullatoViewModel
    {
        [Required(ErrorMessage = "Campo obbligatorio.")]
        public string Codice { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio.")]
        public string Colore { get; set; }
        [DisplayName("XXS/40")]
        public bool isXxsToBeNullified { get; set; }
        [DisplayName("XS/42")]
        public bool isXsToBeNullified { get; set; }
        [DisplayName("S/44")]
        public bool isSToBeNullified { get; set; }
        [DisplayName("M/46")]
        public bool isMToBeNullified { get; set; }
        [DisplayName("L/48")]
        public bool isLToBeNullified { get; set; }
        [DisplayName("XL/50")]
        public bool isXlToBeNullified { get; set; }
        [DisplayName("XXL/52")]
        public bool isXxlToBeNullified { get; set; }
        [DisplayName("XXXL/54")]
        public bool isXxxlToBeNullified { get; set; }
        [DisplayName("Taglia Unica")]
        public bool isTagliaUnicaToBeNullified { get; set; }
        [DisplayName("Annulla tutte le taglie")]
        public bool isAllToBeNullified { get; set; }
    }
}
