//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication4.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Local
    {
        public string ubicacion { get; set; }
        public string descripcion { get; set; }
        public int codLocal { get; set; }
        public Nullable<bool> estaActivo { get; set; }
        public Nullable<int> idProvincia { get; set; }
        public Nullable<int> idRegion { get; set; }
    
        public virtual Region Region { get; set; }
        public virtual Region Region1 { get; set; }
    }
}
