//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrzychodniaMvc.Models.BazaDanych
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class DzienGodzinaPracyLekarza
    {
        public int IdDzienGodzina { get; set; }
        [Required]
        public int DzienTygodnia { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public string GodzinaRozp { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public string GodzinaZak { get; set; }
        [Required]
        public string CzasJednejWizyty { get; set; }
        public int IdLekarza { get; set; }
        public int IdGabinetu { get; set; }
        public List<SelectListItem> listaDniTygodnia = new List<SelectListItem>();
    
        public virtual Lekarz Lekarz { get; set; }
    }
}
