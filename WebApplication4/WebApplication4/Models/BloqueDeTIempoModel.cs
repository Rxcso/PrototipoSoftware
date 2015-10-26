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
        [Display(Name="Fecha Inicio:")]
        public string fechaInicio { get; set; }
        
        [Required]
        [Display(Name = "Fecha Fin:")]
        public string fechaFin { get; set; }
    }
    public class BloqueTiempoListModel
    {
        public List<BloqueDeTiempoModel> ListaBTM { get; set; }
    }
}