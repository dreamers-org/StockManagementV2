using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace loft1Mvc.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the GenericUser class
    public class GenericUser : IdentityUser
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio.")]
        [MaxLength(150, ErrorMessage = "Nome agenzia non valido.")]
        public string AgenziaRappresentanza { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio.")]
        [MaxLength(50, ErrorMessage = "Nome regione non valido.")]
        public string Regione { get; set; }
    }
}
