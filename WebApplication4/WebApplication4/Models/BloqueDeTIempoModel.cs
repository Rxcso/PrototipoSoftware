using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class BloqueDeTiempoModel
    {
        [Required]
        [Display(Name = "Fecha Inicio:")]
        public DateTime fechaInicio { get; set; }

        [Required]
        [Display(Name = "Fecha Fin:")]
        public DateTime fechaFin { get; set; }
        public string razon { get; set; }
    }

    public class BloqueTiempoListModel
    {
        public List<BloqueDeTiempoModel> ListaBTM { get; set; }
        public bool esCorrecto { get; set; }
    }
}