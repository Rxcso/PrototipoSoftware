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
    
    public partial class VentasXFuncion
    {
        public VentasXFuncion()
        {
            this.DetalleVenta = new HashSet<DetalleVenta>();
        }
    
        public int codVen { get; set; }
        public int cantEntradas { get; set; }
        public Nullable<double> subtotal { get; set; }
        public Nullable<int> descuento { get; set; }
        public Nullable<double> total { get; set; }
        public int codFuncion { get; set; }
        public Nullable<bool> hanEntregado { get; set; }
    
        public virtual Ventas Ventas { get; set; }
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; }
        public virtual Funcion Funcion { get; set; }
    }
}
