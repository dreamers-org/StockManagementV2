using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models
{
    public class PackingList
    {
        public Guid Id { get; set; }

        [BindNever]
        public Guid IdArticolo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("2XS/40")]
        public int Xxs { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("XS/42")]
        public int Xs { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("S/44")]
        public int S { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue("0")]
        [DisplayName("M/46")]
        public int M { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("L/48")]
        public int L { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("XL/50")]
        public int Xl { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("2XL/52")]
        public int Xxl { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [DisplayName("3XL/54")]
        public int Xxxl { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        public int TagliaUnica { get; set; }
        [BindNever]
        public DateTime DataInserimento { get; set; }
        [BindNever]
        public string UtenteInserimento { get; set; }
    }
}
