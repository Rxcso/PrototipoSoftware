
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
    
public partial class Promociones
{

    public Nullable<System.DateTime> fechaIni { get; set; }

    public Nullable<System.DateTime> fechaFin { get; set; }

    public Nullable<float> descuento { get; set; }

    public string descripcion { get; set; }

    public Nullable<int> cantAdq { get; set; }

    public Nullable<int> cantComp { get; set; }

    public string modoPago { get; set; }

    public Nullable<bool> estado { get; set; }

    public int codPromo { get; set; }

    public Nullable<int> codBanco { get; set; }

    public Nullable<int> codTipoTarjeta { get; set; }

    public int codEvento { get; set; }



    public virtual Banco Banco { get; set; }

    public virtual TipoTarjeta TipoTarjeta { get; set; }

}

}
