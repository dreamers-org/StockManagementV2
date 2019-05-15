using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class EditRigaOrdineClienteViewModel
    {
        [Key]
        public Guid IdRiga { get; set; }

        [DisplayName("Cod. art")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        public string CodiceArticolo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio.")]
        public string Colore { get; set; }

        [DisplayName("2XS/40")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
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

        [DisplayName("4XL/56")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int Xxxxl { get; set; }

        [DisplayName("Taglia unica")]
        [Required(ErrorMessage = "Campo obbligatorio.")]
        [Range(0, 99, ErrorMessage = "E' possibile inserire solo valori positivi.")]
        public int TagliaUnica { get; set; }
    }
}
