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
    
    public partial class Asientos
    {
        public Asientos()
        {
            this.AsientosXFuncion = new HashSet<AsientosXFuncion>();
        }
    
        public Nullable<int> fila { get; set; }
        public Nullable<int> columna { get; set; }
        public int codAsiento { get; set; }
        public int codZona { get; set; }
    
        public virtual ICollection<AsientosXFuncion> AsientosXFuncion { get; set; }
        public virtual ZonaEvento ZonaEvento { get; set; }
    }
}
