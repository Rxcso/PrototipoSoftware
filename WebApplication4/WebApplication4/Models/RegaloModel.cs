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
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Descripción: ")]
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string descripcion { get; set; }

        [Required]
        [PosNumberAttribute(ErrorMessage = "Debe ser un numero Positivo mayor que cero")]
        [Display(Name = "Puntos: ")]
        public int puntos { get; set; }
    }

    public class RegaloSearchModel
    {

        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }
    }

    public class RegaloListModel
    {
        [Required]
        [Display(Name = "Regalo: ")]
        public int id { get; set; }
    }

    public class PosNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            int getal;
            if (int.TryParse(value.ToString(), out getal))
            {

                if (getal >= 0)
                    return true;
            }
            return false;
        }
    }
}