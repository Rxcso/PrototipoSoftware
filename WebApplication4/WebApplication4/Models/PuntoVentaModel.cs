using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class PuntoVentaModel
    {
        [Required]
        [Display(Name = "Ubicacion: ")]
        public String ubicacion { get; set; }

        [Required]
        [Display(Name = "Direccion MAC: ")]
        public String mac { get; set; }

        [Required]
        [Display(Name = "Provincia: ")]
        public int idProv { get; set; }

        [Required]
        [Display(Name = "Departamento: ")]
        public int idRegion { get; set; }
    }

    public class PuntoVentaSearchModel
    {
        [Required]
        [Display(Name = "Ubicacion: ")]
        public string ubicacion { get; set; }
    }
}