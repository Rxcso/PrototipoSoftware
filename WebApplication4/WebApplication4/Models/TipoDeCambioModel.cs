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
        [Display(Name = "Valor: ")]
        [PosNumberAttribute3(ErrorMessage = "Debe ser un numero Positivo mayor que cero")]
        public double valor { get; set; }
    }

    public class PosNumberAttribute3 : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            double getal;
            if (double.TryParse(value.ToString(), out getal))
            {

                if (getal >= 0)
                    return true;
            }
            return false;
        }
    }
}