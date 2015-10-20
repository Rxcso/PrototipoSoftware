using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class EventoModel
    {
        //PESTAÑA DATOS GENERALES
        [Required]
        [Display(Name="Nombre:")]
        public string nombre { get; set; }

        [Required]
        [Display(Name="Organizador:")]
        public int idOrganizador { get; set; }

        [Required]
        [Display(Name = "Tipo:")]
        public int idCategoria { get; set; }

        [Required]
        [Display(Name = "Categoría:")]
        public int idSubCat { get; set; }

        [Display(Name = "Local:")]
        public int idLocal { get; set; }

        [Display(Name = "Dirección:")]
        public string lugar { get; set; }
        
        [Required]
        [Display(Name = "Departamento:")]
        public int idRegion { get; set; }

        [Required]
        [Display(Name = "Provincia:")]
        public int idProv { get; set; }

        [Required]
        [Display(Name = "Descripcion:")]
        public string descripcion { get; set; }
        //TERMINA DATOS GENERALES
        //public Nullable<double> penalidadXcancelacion { get; set; }

        //public Nullable<double> penalidadXpostergacion { get; set; }

        //public Nullable<double> porccomision { get; set; }

        //public Nullable<bool> esUnico { get; set; }

        

       
        //public Nullable<int> idPromotor { get; set; }


        [Required]
        [Display(Name="Imagen")]
        public string ImagenEvento { get; set; }

        [Required]
        [Display(Name="Imagen Destacada")]
        public string ImagenDestacado { get; set; }

        //public DateTime fecha_inicio { get; set; }
        //Se calcula

        //public DateTime fecha_fin { get; set; }
        //Se calcula

        //public string estado { get; set; }
        //empieza como activo

        //calculados despues
        //public Nullable<double> monto_transferir { get; set; }
        //public Nullable<double> monto_adeudado { get; set; }
    }
}