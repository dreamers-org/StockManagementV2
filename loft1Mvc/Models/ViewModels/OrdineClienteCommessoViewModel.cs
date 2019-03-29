using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class OrdineClienteCommessoViewModel
    {
        [Required(ErrorMessage = "Campo obbligatorio")]
        public string NomeCliente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string Indirizzo { get; set; }

        [DisplayName("Consegna")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataConsegna { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string Rappresentante { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string IndirizzoCliente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string EmailCliente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string CodiceArticolo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string ColoreArticolo { get; set; }

        [DisplayName("2XS/40")]
        public int? Xxs { get; set; }

        [DisplayName("XS/42")]
        public int? Xs { get; set; }

        [DisplayName("S/44")]
        public int? S { get; set; }

        [DisplayName("M/46")]
        public int? M { get; set; }

        [DisplayName("L/48")]
        public int? L { get; set; }

        [DisplayName("XL/50")]
        public int? Xl { get; set; }

        [DisplayName("2XL/52")]
        public int? Xxl { get; set; }

        [DisplayName("3XL/54")]
        public int? Xxxl { get; set; }
    }
}
