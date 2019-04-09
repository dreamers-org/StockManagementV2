using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Models.ViewModels
{
    public class AccettazioneCondizioni
    {
        public Guid IdOrdine { get; set; }
        public string Cliente { get; set; }
        public string DataOrdine { get; set; }
        public IFormFile File { get; set; }
    }
}
