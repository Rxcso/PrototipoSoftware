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
    
    public partial class LogDevoluciones
    {
        public int codLog { get; set; }
        public int codDetalleVenta { get; set; }
        public string codVendedor { get; set; }
        public Nullable<int> cantEntradas { get; set; }
        public Nullable<System.DateTime> fechaDev { get; set; }
        public Nullable<double> montoDev { get; set; }
    
        public virtual DetalleVenta DetalleVenta { get; set; }
        public virtual CuentaUsuario CuentaUsuario { get; set; }
    }
}
