using System;
using System.Collections.Generic;

namespace loft1Mvc.Models
{
    public partial class PackingList
    {
        public int Id { get; set; }
        public int Codice { get; set; }
        public string Fornitore { get; set; }
        public string Variante { get; set; }
        public int? Xxxs { get; set; }
        public int? Xxs { get; set; }
        public int? Xs { get; set; }
        public int? S { get; set; }
        public int? M { get; set; }
        public int? L { get; set; }
        public int? Xl { get; set; }
        public int? Xxl { get; set; }
        public int? Xxxl { get; set; }
        public int? Xxxxl { get; set; }
		public string attr1 { get; set; }
		public string attr2 { get; set; }
	}
}
