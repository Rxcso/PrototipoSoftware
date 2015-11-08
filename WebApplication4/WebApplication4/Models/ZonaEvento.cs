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
    
    public partial class ZonaEvento
    {
        public ZonaEvento()
        {
            this.Asientos = new HashSet<Asientos>();
            this.PrecioEvento = new HashSet<PrecioEvento>();
            this.ZonaxFuncion = new HashSet<ZonaxFuncion>();
        }
    
        public int codZona { get; set; }
        public int codEvento { get; set; }
        public string nombre { get; set; }
        public int aforo { get; set; }
        public bool tieneAsientos { get; set; }
        public int cantFilas { get; set; }
        public int cantColumnas { get; set; }
    
        public virtual ICollection<Asientos> Asientos { get; set; }
        public virtual ICollection<PrecioEvento> PrecioEvento { get; set; }
        public virtual Eventos Eventos { get; set; }
        public virtual ICollection<ZonaxFuncion> ZonaxFuncion { get; set; }
    }
}
