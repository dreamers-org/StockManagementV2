using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagement.Models
{
    public partial class RigaOrdineFornitore
    {
        [BindNever]
        public Guid Id { get; set; }
        [BindNever]
        public Guid IdArticolo { get; set; }
        [BindNever]
        public Guid IdFornitore { get; set; }
        [DisplayName("2XS/40")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int Xxs { get; set; }
        [DisplayName("XS/42")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int Xs { get; set; }
        [DisplayName("S/44")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int S { get; set; }
        [DisplayName("M/46")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int M { get; set; }
        [DisplayName("L/48")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int L { get; set; }
        [DisplayName("XL/50")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int Xl { get; set; }
        [DisplayName("2XL/52")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int Xxl { get; set; }
        [DisplayName("3XL/54")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int Xxxl { get; set; }
        [DisplayName("4XL/56")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int Xxxxl { get; set; }
        [DisplayName("T.U.")]
        [Required(ErrorMessage = "Obbligatorio")]
        public int TagliaUnica { get; set; }
        [BindNever]
        public string UtenteModifica { get; set; }
        [BindNever]
        public string UtenteInserimento { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [BindNever]
        public DateTime? DataModifica { get; set; }
        [BindNever]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataInserimento { get; set; }

        public virtual Articolo IdNavigation { get; set; }

        [ForeignKey("IdFornitore")]
        public virtual Fornitore IdFornitoreNavigation { get; set; }
    }
}
