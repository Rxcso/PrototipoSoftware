
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
    
public partial class Banco
{

    public Banco()
    {

        this.Promociones = new HashSet<Promociones>();

    }


    public int codigo { get; set; }

    public string nombre { get; set; }



    public virtual ICollection<Promociones> Promociones { get; set; }

}

}
