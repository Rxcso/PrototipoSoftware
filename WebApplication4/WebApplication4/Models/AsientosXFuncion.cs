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
    
    public partial class AsientosXFuncion
    {
        public int codAsiento { get; set; }
        public int codFuncion { get; set; }
        public string estado { get; set; }
        public Nullable<int> codDetalleVenta { get; set; }
        public Nullable<int> TipoDocAsociado { get; set; }
        public string DocAsociado { get; set; }
        public Nullable<double> PrecioPagado { get; set; }
        public byte[] concurrencia { get; set; }
    
        public virtual Asientos Asientos { get; set; }
        public virtual Funcion Funcion { get; set; }
    }
}
