using System;
using System.ComponentModel;

namespace StockManagement.Models.ViewModels
{
    public class ViewPackingListViewModel
    {

        public string Codice { get; set; }
        public Guid IdPackingList { get; set; }
        public string Colore { get; set; }

        [DefaultValue(0)]
        [DisplayName("2XS/40")]
        public int Xxs { get; set; }
        [DefaultValue(0)]
        [DisplayName("XS/42")]
        public int Xs { get; set; }
        [DefaultValue(0)]
        [DisplayName("S/44")]
        public int S { get; set; }
        [DefaultValue(0)]
        [DisplayName("M/46")]
        public int M { get; set; }
        [DefaultValue(0)]
        [DisplayName("L/48")]
        public int L { get; set; }
        [DefaultValue(0)]
        [DisplayName("XL/50")]
        public int Xl { get; set; }
        [DefaultValue(0)]
        [DisplayName("2XL/52")]
        public int Xxl { get; set; }
        [DefaultValue(0)]
        [DisplayName("3XL/54")]
        public int Xxxl { get; set; }
        [DefaultValue(0)]
        [DisplayName("4XL/56")]
        public int Xxxxl { get; set; }
        [DefaultValue(0)]
        public int TagliaUnica { get; set; }
    }
}
