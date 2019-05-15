using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
    public partial class OrdineCliente
    {
        public Guid Id { get; set; }
        [DisplayName("Rappresentante")]
        public Guid IdRappresentante { get; set; }
        [DisplayName("Cliente")]
        public Guid IdCliente { get; set; }
        [DisplayName("Consegna")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DataConsegna { get; set; }
        public Guid? IdTipoPagamento { get; set; }
        public string Note { get; set; }
        public bool Completato { get; set; }
        public bool Pagato { get; set; }
        public DateTime DataInserimento { get; set; }
        public DateTime? DataModifica { get; set; }
        public string UtenteInserimento { get; set; }
        public string UtenteModifica { get; set; }
        public virtual Cliente IdNavigation { get; set; }
        public virtual TipoPagamento IdPagamentoNavigation { get; set; }
        public virtual RigaOrdineCliente RigaOrdineCliente { get; set; }
        public bool Spedito { get; set; }
        [DisplayName("Spedito in parte")]
        public bool SpeditoInParte { get; set; }
        public bool Letto { get; set; }
        public bool Stampato { get; set; }
        [BindNever]
        public string RandomNumber { get; set; }
    }
}
