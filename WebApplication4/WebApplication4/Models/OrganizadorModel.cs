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
        public string Email { get; set; }

        [Required]
        [Range(1, 3)]
        [Display(Name = "Tipo de Doc.: ")]
        public int tipoDoc { get; set; }

        [Required]
        [Display(Name = "# Doc: ")]
        public string codDoc { get; set; }

        [Required]
        [Display(Name = "Nombre Organizador: ")]
        public string nombre { get; set; }

        [Display(Name = "Telefono")]
        public string telefono { get; set; }
    }

    public class OrganizadorSearchModel
    {

        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }
    }

}