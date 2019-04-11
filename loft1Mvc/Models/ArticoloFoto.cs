using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models
{
    public class ArticoloFoto
    {
        [Required(ErrorMessage = "Obbligatorio.")]
        public Guid Id { get; set; }
        [DisplayName("Cod.")]
        [Required(ErrorMessage = "Obbligatorio.")]
        public Guid IdArticolo { get; set; }
        [DisplayName("Foto")]
        public byte[] Foto { get; set; }
        public virtual Articolo IdNavigation { get; set; }
    }
}
