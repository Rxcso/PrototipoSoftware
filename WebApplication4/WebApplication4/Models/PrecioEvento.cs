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
    
    public partial class PrecioEvento
    {
        public PrecioEvento()
        {
            this.DetalleVenta = new HashSet<DetalleVenta>();
        }
    
        public int codPrecioEvento { get; set; }
        public Nullable<double> precio { get; set; }
        public Nullable<int> codPeriodoVenta { get; set; }
        public Nullable<int> codZonaEvento { get; set; }
    
        public virtual PeriodoVenta PeriodoVenta { get; set; }
        public virtual ZonaEvento ZonaEvento { get; set; }
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; }
    }
}
