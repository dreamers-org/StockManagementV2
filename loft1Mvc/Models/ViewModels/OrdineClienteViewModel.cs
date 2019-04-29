using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class OrdineClienteViewModel:OrdineCliente
    {
        [Required(ErrorMessage = "Campo obbligatorio")]
		public string NomeCliente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string IndirizzoCliente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [EmailAddress(ErrorMessage = "Inserire una mail valida.")]
        public string EmailCliente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string CodiceArticolo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string ColoreArticolo { get; set; }

        [DisplayName("2XS/40")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0,99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int Xxs { get; set; }

        [DisplayName("XS/42")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int Xs { get; set; }

        [DisplayName("S/44")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int S { get; set; }

        [DisplayName("M/46")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int M { get; set; }

        [DisplayName("L/48")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int L { get; set; }

        [DisplayName("XL/50")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int Xl { get; set; }

        [DisplayName("2XL/52")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int Xxl { get; set; }

        [DisplayName("3XL/54")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int Xxxl { get; set; }

        [DisplayName("Taglia unica")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int TagliaUnica { get; set; }
    }
}
