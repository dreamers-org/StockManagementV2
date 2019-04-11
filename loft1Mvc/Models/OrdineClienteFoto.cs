using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models
{
    public class OrdineClienteFoto
    {
        [Required(ErrorMessage = "Obbligatorio.")]
        public Guid Id { get; set; }
        [DisplayName("Cod.")]
        [Required(ErrorMessage = "Obbligatorio.")]
        public Guid IdOrdine { get; set; }
        [DisplayName("Foto")]
        public byte[] Foto { get; set; }
        public virtual OrdineCliente IdNavigation { get; set; }
    }
}
