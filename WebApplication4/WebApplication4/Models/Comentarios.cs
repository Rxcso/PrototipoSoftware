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
    
    public partial class Comentarios
    {
        public int codComentario { get; set; }
        public string contenido { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public int codEvento { get; set; }
        public string usuario { get; set; }
    
        public virtual Eventos Eventos { get; set; }
        public virtual CuentaUsuario CuentaUsuario { get; set; }
    }
}
