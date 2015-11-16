using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class TipoDeCambioModel
    {
        [Required]
        [RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "Tipo de Cambio no debe tener mas de 2 decimales")]
        [Display(Name = "Valor: ")]
        public float valor { get; set; }
    }
}