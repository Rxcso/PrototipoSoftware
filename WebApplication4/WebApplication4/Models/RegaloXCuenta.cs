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
    
    public partial class RegaloXCuenta
    {
        public System.DateTime fechaRecojo { get; set; }
        public int idRegalo { get; set; }
        public string usuario { get; set; }
    
        public virtual Regalo Regalo { get; set; }
        public virtual CuentaUsuario CuentaUsuario { get; set; }
    }
}
