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
    
    public partial class Categoria
    {
        public Categoria()
        {
            this.Categoria1 = new HashSet<Categoria>();
            this.CuentaUsuario = new HashSet<CuentaUsuario>();
        }
    
        public int idCategoria { get; set; }
        public Nullable<int> idCatPadre { get; set; }
        public string descripcion { get; set; }
        public string nombre { get; set; }
        public Nullable<int> nivel { get; set; }
        public Nullable<int> activo { get; set; }
    
        public virtual ICollection<Categoria> Categoria1 { get; set; }
        public virtual Categoria Categoria2 { get; set; }
        public virtual ICollection<CuentaUsuario> CuentaUsuario { get; set; }
    }
}
