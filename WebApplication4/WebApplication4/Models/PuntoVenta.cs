
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
    
public partial class PuntoVenta
{

    public PuntoVenta()
    {

        this.Turno = new HashSet<Turno>();

    }


    public int codPuntoVenta { get; set; }

    public string ubicacion { get; set; }

    public Nullable<bool> estaActivo { get; set; }

    public string dirMAC { get; set; }



    public virtual ICollection<Turno> Turno { get; set; }

}

}
