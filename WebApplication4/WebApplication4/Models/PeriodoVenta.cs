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
    
    public partial class PeriodoVenta
    {
        public PeriodoVenta()
        {
            this.PrecioEvento = new HashSet<PrecioEvento>();
        }
    
        public int idPerVent { get; set; }
        public Nullable<System.DateTime> fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFin { get; set; }
        public Nullable<int> codEvento { get; set; }
        public Nullable<int> idPrecioEvento { get; set; }
    
        public virtual Eventos Eventos { get; set; }
        public virtual ICollection<PrecioEvento> PrecioEvento { get; set; }
        public virtual PrecioEvento PrecioEvento1 { get; set; }
    }
}
