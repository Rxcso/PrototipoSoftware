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
    
    public partial class TipoTarjeta
    {
        public TipoTarjeta()
        {
            this.Promociones = new HashSet<Promociones>();
        }
    
        public int idTipoTar { get; set; }
        public string nombre { get; set; }
    
        public virtual ICollection<Promociones> Promociones { get; set; }
    }
}
