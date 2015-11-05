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
    
    public partial class CuentaUsuario
    {
        public CuentaUsuario()
        {
            this.RegaloXCuenta = new HashSet<RegaloXCuenta>();
            this.Categoria = new HashSet<Categoria>();
            this.Turno = new HashSet<Turno>();
            this.Ventas = new HashSet<Ventas>();
            this.Ventas1 = new HashSet<Ventas>();
            this.Comentarios = new HashSet<Comentarios>();
        }
    
        public string usuario { get; set; }
        public string tipoUsuario { get; set; }
        public string correo { get; set; }
        public string contrasena { get; set; }
        public bool estado { get; set; }
        public Nullable<int> tipoDoc { get; set; }
        public string codDoc { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string telMovil { get; set; }
        public string sexo { get; set; }
        public Nullable<System.DateTime> fechaNac { get; set; }
        public int puntos { get; set; }
        public int codPerfil { get; set; }
    
        public virtual ICollection<RegaloXCuenta> RegaloXCuenta { get; set; }
        public virtual ICollection<Categoria> Categoria { get; set; }
        public virtual ICollection<Turno> Turno { get; set; }
        public virtual ICollection<Ventas> Ventas { get; set; }
        public virtual ICollection<Ventas> Ventas1 { get; set; }
        public virtual ICollection<Comentarios> Comentarios { get; set; }
    }
}
