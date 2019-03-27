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
        [Range(0,999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("2XS/40")]
        public int Xxs { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("XS/42")]
        public int Xs { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("S/44")]
        public int S { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("M/46")]
        public int M { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("L/48")]
        public int L { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("XL/50")]
        public int Xl { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
        [DisplayName("2XL/52")]
        public int Xxl { get; set; }
        [Required(ErrorMessage = "Campo obbligatorio")]
        [DefaultValue(0)]
        [Range(0, 999, ErrorMessage = "Non possono essere inseriti valori negativi")]
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
