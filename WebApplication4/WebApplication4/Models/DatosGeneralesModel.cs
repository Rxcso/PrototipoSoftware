using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class DatosGeneralesModel
    {
        [Required]
        [Display(Name = "Nombre:")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Organizador:")]
        public int idOrganizador { get; set; }

        [Required]
        [Display(Name = "Tipo:")]
        public int idCategoria { get; set; }

        [Display(Name = "Categoría:")]
        public int idSubCat { get; set; }
        
        [Display(Name = "Local:")]
        public int idLocal { get; set; }

        [Display(Name = "Direccion")]
        public string Direccion { get; set; }

        [Display(Name = "Departamento:")]
        public int idRegion { get; set; }

        [Display(Name = "Provincia:")]
        public int idProv { get; set; }

        [Display(Name = "Descripcion:")]
        public string descripcion { get; set; }
    }
}