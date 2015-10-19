using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class LocalModel
    {
        [Required]
        [Display(Name = "Aforo: ")]
        public int aforo { get; set; }

        [Required]
        [Display(Name = "Descripcion: ")]
        public string descripcion { get; set; }

        [Required]
        [Display(Name = "Ubicacion: ")]
        public String ubicacion { get; set; }

        [Required]
        [Display(Name = "Provincia: ")]
        public int provincia { get; set; }

        [Required]
        [Display(Name = "Departamento: ")]
        public int departamento { get; set; }
    }
}