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
    
    public partial class Organizador
    {
        public Organizador()
        {
            this.Eventos = new HashSet<Eventos>();
            this.Pago = new HashSet<Pago>();
        }
    
        public int codOrg { get; set; }
        public string nombOrg { get; set; }
        public Nullable<int> tipoDoc { get; set; }
        public string codDoc { get; set; }
        public string estadoOrg { get; set; }
        public string correo { get; set; }
        public string telefOrg { get; set; }
    
        public virtual ICollection<Eventos> Eventos { get; set; }
        public virtual ICollection<Pago> Pago { get; set; }
    }
}
