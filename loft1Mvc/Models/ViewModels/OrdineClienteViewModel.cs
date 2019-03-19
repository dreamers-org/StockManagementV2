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
        [Required]
		public string NomeCliente { get; set; }

        [Required]
        public string IndirizzoCliente { get; set; }

        [Required]
        public string EmailCliente { get; set; }

        [Required]
        public string CodiceArticolo { get; set; }

        [Required]
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
