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
    
    public partial class ZonaEvento
    {
        public ZonaEvento()
        {
            this.Asientos = new HashSet<Asientos>();
        }
    
        public int codZona { get; set; }
        public int codEvento { get; set; }
        public string nombre { get; set; }
        public int aforo { get; set; }
        public bool tieneAsientos { get; set; }
        public Nullable<int> cantFilas { get; set; }
        public Nullable<int> cantColumnas { get; set; }
    
        public virtual ICollection<Asientos> Asientos { get; set; }
        public virtual Eventos Eventos { get; set; }
    }
}
