//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
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
