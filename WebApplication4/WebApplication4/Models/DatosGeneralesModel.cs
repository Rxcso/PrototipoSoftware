using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class DatosGeneralesModel
    {
        [Required(ErrorMessage = "El evento debe tener un nombre.")]
        [Display(Name = "Nombre:")]
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El evento debe tener un organizador.")]
        [Display(Name = "Organizador:")]
        public int idOrganizador { get; set; }

        [Required(ErrorMessage = "El evento debe ser de un tipo.")]
        [Display(Name = "Categoria:")]
        public int idCategoria { get; set; }
        
        [Display(Name = "Subcategoria:")]
        public int idSubCat { get; set; }

        [Display(Name = "Direccion:")]
        [StringLength(100, ErrorMessage = "Maxima longitud de 100 caracteres.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El evento debe tener una ubicacion.")]
        [Display(Name = "Departamento:")]
        public int idRegion { get; set; }

        [Display(Name = "Provincia:")]
        public int idProv { get; set; }

        [Required]
        [Display(Name = "Descripcion:")]
        [StringLength(200,ErrorMessage="Maxima longitud de 200 caracteres.")]
        public string descripcion { get; set; }

        [Display(Name = "Local:")]
        public int Local { get; set; }
    }
}