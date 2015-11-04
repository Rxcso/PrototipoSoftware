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
    
    public partial class Eventos
    {
        public Eventos()
        {
            this.Comentarios = new HashSet<Comentarios>();
            this.Pago = new HashSet<Pago>();
            this.PeriodoVenta = new HashSet<PeriodoVenta>();
            this.ZonaEvento = new HashSet<ZonaEvento>();
            this.Funcion = new HashSet<Funcion>();
        }
    
        public int codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public string estado { get; set; }
        public Nullable<double> monto_transferir { get; set; }
        public Nullable<double> monto_adeudado { get; set; }
        public string direccion { get; set; }
        public Nullable<double> penalidadXcancelacion { get; set; }
        public Nullable<double> penalidadXpostergacion { get; set; }
        public Nullable<double> porccomision { get; set; }
        public Nullable<int> idCategoria { get; set; }
        public Nullable<int> idSubcategoria { get; set; }
        public Nullable<int> idRegion { get; set; }
        public Nullable<int> idProvincia { get; set; }
        public Nullable<int> idPromotor { get; set; }
        public Nullable<int> idOrganizador { get; set; }
        public Nullable<System.DateTime> fechaRegistro { get; set; }
        public Nullable<System.DateTime> fechaUltModificacion { get; set; }
        public string ImagenEvento { get; set; }
        public string ImagenDestacado { get; set; }
        public string ImagenSitios { get; set; }
        public Nullable<int> maxReservas { get; set; }
        public Nullable<double> montoFijoVentaEntrada { get; set; }
        public Nullable<bool> tieneBoletoElectronico { get; set; }
        public Nullable<bool> permiteReserva { get; set; }
        public Nullable<int> puntosAlCliente { get; set; }
        public Nullable<int> idLocal { get; set; }
    
        public virtual ICollection<Comentarios> Comentarios { get; set; }
        public virtual Organizador Organizador { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Pago> Pago { get; set; }
        public virtual ICollection<PeriodoVenta> PeriodoVenta { get; set; }
        public virtual ICollection<ZonaEvento> ZonaEvento { get; set; }
        public virtual ICollection<Funcion> Funcion { get; set; }
    }
}
