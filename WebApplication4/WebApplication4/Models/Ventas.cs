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
    
    public partial class Ventas
    {
        public Ventas()
        {
            this.VentasXFuncion = new HashSet<VentasXFuncion>();
        }
    
        public int codVen { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> cantAsientos { get; set; }
        public Nullable<double> montoEfectivoSoles { get; set; }
        public Nullable<double> montoEfectivoDolares { get; set; }
        public Nullable<double> montoCreditoSoles { get; set; }
        public string modalidad { get; set; }
        public Nullable<int> tipoDoc { get; set; }
        public string codDoc { get; set; }
        public string cliente { get; set; }
        public string vendedor { get; set; }
        public string Estado { get; set; }
        public Nullable<double> MontoTotalSoles { get; set; }
    
        public virtual CuentaUsuario CuentaUsuario { get; set; }
        public virtual CuentaUsuario CuentaUsuario1 { get; set; }
        public virtual ICollection<VentasXFuncion> VentasXFuncion { get; set; }
    }
}
