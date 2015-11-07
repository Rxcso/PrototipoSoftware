using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class OrganizadorModel
    {


        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electronico: ")]
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string Email { get; set; }

        [Required]
        [Range(1, 3)]
        [Display(Name = "Tipo de Doc.: ")]
        public int tipoDoc { get; set; }

        [Required]
        [Display(Name = "# Doc: ")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Numero de documento debe ser numerico.")]
        [StringLength(20, ErrorMessage = "Maxima longitud de 20 caracteres.")]
        public string codDoc { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Maxima longitud de 50 caracteres.")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Nombre no debe ser alfanumerico.")]
        [Display(Name = "Nombre Organizador: ")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Telefono")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefono Movil debe ser numerico.")]
        [StringLength(20, ErrorMessage = "Maxima longitud de 20 caracteres.")]
        public string telefono { get; set; }
    }

    public class OrganizadorSearchModel
    {
        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }
    }

}