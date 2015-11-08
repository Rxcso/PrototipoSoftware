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
        [PosNumberAttribute(ErrorMessage = "Debe ser un numero Positivo mayor que cero")]
        [Display(Name = "Aforo: ")]
        public int aforo { get; set; }

        [Required]
        [Display(Name = "Descripcion: ")]
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string descripcion { get; set; }

        [Required]
        [Display(Name = "Ubicacion: ")]
        [StringLength(50, ErrorMessage = "Maxima longitud de 50 caracteres.")]
        public String ubicacion { get; set; }

        [Required]
        [Display(Name = "Provincia: ")]
        public int idProv { get; set; }

        [Required]
        [Display(Name = "Departamento: ")]
        public int idRegion { get; set; }
    }

    public class LocalEditModel
    {
        [Required]
        [PosNumberAttribute(ErrorMessage = "Debe ser un numero Positivo mayor que cero")]
        [Display(Name = "Aforo: ")]
        public int aforo { get; set; }

        [Required]
        [Display(Name = "Descripcion: ")]
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string descripcion { get; set; }

        [Required]
        [Display(Name = "Ubicacion: ")]
        [StringLength(50, ErrorMessage = "Maxima longitud de 50 caracteres.")]
        public String ubicacion { get; set; }

        [Required]
        [Display(Name = "Provincia: ")]
        public int idProv { get; set; }

        [Required]
        [Display(Name = "Departamento: ")]
        public int idRegion { get; set; }
    }

    public class LocalSearchModel
    {
        [Display(Name = "Descripcion: ")]
        public string nombre { get; set; }

        public int departamento { get; set; }
    }

    public class PosNumberAttribute2 : ValidationAttribute
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