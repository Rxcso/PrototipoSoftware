
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
    
public partial class CuentaUsuario
{

    public CuentaUsuario()
    {

        this.RegaloXCuenta = new HashSet<RegaloXCuenta>();

        this.Turno = new HashSet<Turno>();

        this.Categoria = new HashSet<Categoria>();

    }


    public string usuario { get; set; }

    public string tipoUsuario { get; set; }

    public string correo { get; set; }

    public string contrasena { get; set; }

    public Nullable<bool> estado { get; set; }

    public Nullable<int> tipoDoc { get; set; }

    public string codDoc { get; set; }

    public string nombre { get; set; }

    public string apellido { get; set; }

    public string direccion { get; set; }

    public string telefono { get; set; }

    public string telMovil { get; set; }

    public string sexo { get; set; }

    public Nullable<System.DateTime> fechaNac { get; set; }

    public Nullable<int> puntos { get; set; }

    public int codPerfil { get; set; }



    public virtual ICollection<RegaloXCuenta> RegaloXCuenta { get; set; }

    public virtual ICollection<Turno> Turno { get; set; }

    public virtual ICollection<Categoria> Categoria { get; set; }

}

}
