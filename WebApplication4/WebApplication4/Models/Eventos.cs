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
        public int codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public string estado { get; set; }
        public Nullable<double> monto_transferir { get; set; }
        public Nullable<double> monto_adeudado { get; set; }
        public string lugar { get; set; }
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
    
        public virtual Region Region { get; set; }
        public virtual Organizador Organizador { get; set; }
    }
}
