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
    
    public partial class Funcion
    {
        public Funcion()
        {
            this.AsientosXFuncion = new HashSet<AsientosXFuncion>();
            this.VentasXFuncion = new HashSet<VentasXFuncion>();
            this.ZonaxFuncion = new HashSet<ZonaxFuncion>();
        }
    
        public int codFuncion { get; set; }
        public Nullable<System.DateTime> horaIni { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public int codEvento { get; set; }
        public string estado { get; set; }
        public Nullable<System.DateTime> FechaDevolucion { get; set; }
        public Nullable<int> cantDiasDevolucion { get; set; }
        public string motivoCambio { get; set; }
    
        public virtual ICollection<AsientosXFuncion> AsientosXFuncion { get; set; }
        public virtual ICollection<VentasXFuncion> VentasXFuncion { get; set; }
        public virtual ICollection<ZonaxFuncion> ZonaxFuncion { get; set; }
        public virtual Eventos Eventos { get; set; }
    }
}
