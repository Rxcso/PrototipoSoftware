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
    
    public partial class Funcion
    {
        public Funcion()
        {
            this.VentasXFuncion = new HashSet<VentasXFuncion>();
        }
    
        public int codFuncion { get; set; }
        public Nullable<System.DateTime> horaIni { get; set; }
        public Nullable<System.DateTime> horaFin { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public int codEvento { get; set; }
        public string estado { get; set; }
        public Nullable<System.DateTime> FechaDevolucion { get; set; }
        public Nullable<int> cantDiasDevolucion { get; set; }
    
        public virtual ICollection<VentasXFuncion> VentasXFuncion { get; set; }
        public virtual Eventos Eventos { get; set; }
    }
}
