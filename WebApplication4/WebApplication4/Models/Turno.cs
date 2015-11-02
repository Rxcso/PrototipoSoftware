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
    
    public partial class Turno
    {
        public int codPuntoVenta { get; set; }
        public int codTurnoSis { get; set; }
        public string estado { get; set; }
        public string usuario { get; set; }
        public System.DateTime fecha { get; set; }
        public Nullable<double> MontoInicioSoles { get; set; }
        public Nullable<double> MontoInicioDolares { get; set; }
        public Nullable<double> MontoFinSoles { get; set; }
        public Nullable<double> MontoFinDolares { get; set; }
        public string estadoCaja { get; set; }
    
        public virtual CuentaUsuario CuentaUsuario { get; set; }
        public virtual PuntoVenta PuntoVenta { get; set; }
        public virtual TurnoSistema TurnoSistema { get; set; }
    }
}
