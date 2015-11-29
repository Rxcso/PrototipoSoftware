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
        [Display(Name = "Categoría:")]
        public int idCategoria { get; set; }
        
        [Display(Name = "Subcategoría:")]
        public int idSubCat { get; set; }

        [Display(Name = "Dirección:")]
        [StringLength(100, ErrorMessage = "Máxima longitud de 100 caracteres.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El evento debe tener una ubicación.")]
        [Display(Name = "Dpto:")]
        public int idRegion { get; set; }

        [Display(Name = "Provincia:")]
        public int idProv { get; set; }

        [Required]
        [Display(Name = "Descripción:")]
        [StringLength(3000,ErrorMessage="Máxima longitud de 200 caracteres.")]
        public string descripcion { get; set; }

        [Display(Name = "Local:")]
        public int Local { get; set; }
    }
}