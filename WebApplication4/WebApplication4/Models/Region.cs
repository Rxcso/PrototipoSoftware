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
    
    public partial class Region
    {
        public Region()
        {
            this.Region1 = new HashSet<Region>();
            this.PuntoVenta = new HashSet<PuntoVenta>();
            this.PuntoVenta1 = new HashSet<PuntoVenta>();
            this.Local = new HashSet<Local>();
            this.Local1 = new HashSet<Local>();
            this.Eventos = new HashSet<Eventos>();
        }
    
        public int idRegion { get; set; }
        public Nullable<int> idRegPadre { get; set; }
        public string nombre { get; set; }
    
        public virtual ICollection<Region> Region1 { get; set; }
        public virtual Region Region2 { get; set; }
        public virtual ICollection<PuntoVenta> PuntoVenta { get; set; }
        public virtual ICollection<PuntoVenta> PuntoVenta1 { get; set; }
        public virtual ICollection<Local> Local { get; set; }
        public virtual ICollection<Local> Local1 { get; set; }
        public virtual ICollection<Eventos> Eventos { get; set; }
    }
}
