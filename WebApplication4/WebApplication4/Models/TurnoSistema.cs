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
    
    public partial class TurnoSistema
    {
        public TurnoSistema()
        {
            this.Turno = new HashSet<Turno>();
        }
    
        public int codTurnoSis { get; set; }
        public Nullable<System.DateTime> horIni { get; set; }
        public Nullable<System.DateTime> horFin { get; set; }
        public Nullable<int> activo { get; set; }
    
        public virtual ICollection<Turno> Turno { get; set; }
    }
}