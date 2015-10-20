using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class RegaloModel
    {

        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Descripcion: ")]
        public string descripcion { get; set; }

        [Required]
        [Display(Name = "Puntos: ")]
        public int puntos { get; set; }
    }

    public class RegaloSearchModel
    {

        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }
    }
}