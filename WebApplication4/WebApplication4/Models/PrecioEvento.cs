//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
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
