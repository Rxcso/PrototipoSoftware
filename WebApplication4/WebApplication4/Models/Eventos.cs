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
    
    public partial class Eventos
    {
        public Eventos()
        {
            this.Pago = new HashSet<Pago>();
            this.Funcion = new HashSet<Funcion>();
            this.PeriodoVenta = new HashSet<PeriodoVenta>();
            this.ZonaEvento = new HashSet<ZonaEvento>();
        }
    
        public int codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public string estado { get; set; }
        public Nullable<double> monto_transferir { get; set; }
        public Nullable<double> monto_adeudado { get; set; }
        public Nullable<double> penalidadXcancelacion { get; set; }
        public Nullable<double> penalidadXpostergacion { get; set; }
        public Nullable<double> porccomision { get; set; }
        public Nullable<bool> esUnico { get; set; }
        public Nullable<int> idCategoria { get; set; }
        public Nullable<int> idRegion { get; set; }
        public Nullable<int> idPromotor { get; set; }
        public Nullable<int> idOrganizador { get; set; }
        public Nullable<System.DateTime> fechaRegistro { get; set; }
        public Nullable<System.DateTime> fechaUltModificacion { get; set; }
        public string ImagenEvento { get; set; }
        public string ImagenDestacado { get; set; }
        public string ImagenSitios { get; set; }
        public Nullable<int> idSubcategoria { get; set; }
        public Nullable<int> idProvincia { get; set; }
        public string direccion { get; set; }
    
        public virtual Region Region { get; set; }
        public virtual Organizador Organizador { get; set; }
        public virtual ICollection<Pago> Pago { get; set; }
        public virtual ICollection<Funcion> Funcion { get; set; }
        public virtual ICollection<PeriodoVenta> PeriodoVenta { get; set; }
        public virtual ICollection<ZonaEvento> ZonaEvento { get; set; }
    }
}
