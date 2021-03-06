﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Models
{
	public partial class Articolo
    {
        public Guid Id { get; set; }
		[DisplayName("Cod.")]
        [StringLength(100, ErrorMessage = "Valore inserito troppo lungo.")]
        [Required(ErrorMessage = "Obbligatorio.")]
		public string Codice { get; set; }
		[DisplayName("Descr.")]
        [StringLength(100, ErrorMessage = "Valore inserito troppo lungo.")]
        [Required(ErrorMessage = "Obbligatorio.")]
		public string Descrizione { get; set; }
		[DisplayName("Fornitore")]
        [Required(ErrorMessage = "Obbligatorio.")]
		public Guid IdFornitore { get; set; }
		[DisplayName("Colore")]
        [StringLength(100, ErrorMessage = "Valore inserito troppo lungo.")]
        [Required(ErrorMessage = "Obbligatorio.")]
		public string Colore { get; set; }
        [DisplayName("2XS/40")]
        public bool Xxs { get; set; }
        [BindNever]
        public bool isXxsActive { get; set; }
        [DisplayName("XS/42")]
        public bool Xs { get; set; }
        [BindNever]
        public bool isXsActive { get; set; }
        [DisplayName("S/44")]
        public bool S { get; set; }
        [BindNever]
        public bool isSActive { get; set; }
        [DisplayName("M/46")]
        public bool M { get; set; }
        [BindNever]
        public bool isMActive { get; set; }
        [DisplayName("L/48")]
        public bool L { get; set; }
        [BindNever]
        public bool isLActive { get; set; }
        [DisplayName("XL/50")]
        public bool Xl { get; set; }
        [BindNever]
        public bool isXlActive { get; set; }
        [DisplayName("2XL/52")]
        public bool Xxl { get; set; }
        [BindNever]
        public bool isXxlActive { get; set; }
        [DisplayName("3XL/54")]
        public bool Xxxl { get; set; }
        [BindNever]
        public bool isXxxlActive { get; set; }
        [DisplayName("4XL/56")]
        public bool Xxxxl { get; set; }
        [BindNever]
        public bool isXxxxlActive { get; set; }
        [DisplayName("Taglia unica")]
        public bool TagliaUnica { get; set; }
        [BindNever]
        public bool isTagliaUnicaActive { get; set; }

        [DisplayName("Tranche consegna")]
		[Required(ErrorMessage = "Obbligatorio.")]
		[DataType(DataType.Date, ErrorMessage = "Inserire una data valida.")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime TrancheConsegna { get; set; }
		[DisplayName("Uomo/donna")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public string Genere { get; set; }
		[DisplayName("Tipo")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public Guid IdTipo { get; set; }
		[DisplayName("Annullato")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public bool Annullato { get; set; }
		[DisplayName("€ Acquisto")]
		[Required(ErrorMessage = "Obbligatorio.")]
        public double PrezzoAcquisto { get; set; }
		[DisplayName("€ Vendita")]
        [Required(ErrorMessage = "Obbligatorio.")]
        public double PrezzoVendita { get; set; }
		[BindNever]
		public string Video { get; set; }
		[DisplayName("Collezione")]
		[Required(ErrorMessage = "Obbligatorio.")]
		public Guid IdCollezione { get; set; }
		[BindNever]
		public DateTime DataInserimento { get; set; }
		[BindNever]
		public DateTime? DataModifica { get; set; }
		[BindNever]
		public string UtenteInserimento { get; set; }
		[BindNever]
		public string UtenteModifica { get; set; }
		[DisplayName("Collezione")]
		public virtual Collezione IdCollezioneNavigation { get; set; }
		[DisplayName("Fornitore")]
		public virtual Fornitore IdFornitoreNavigation { get; set; }
		[DisplayName("Tipo")]
		public virtual Tipo IdTipoNavigation { get; set; }
		public virtual RigaOrdineCliente RigaOrdineCliente { get; set; }
        public virtual RigaOrdineFornitore RigaOrdineFornitore { get; set; }
    }
}
