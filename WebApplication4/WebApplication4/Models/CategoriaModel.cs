using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class CategoriaModel
    {
        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Descripcion: ")]
        public string descripcion { get; set; }
        
        [Required]
        [Display(Name = "Id Padre: ")]
        public int idCatPadre { get; set; }

        public int nivel { get; set; }
    }

    public class CategoriaSearchModel
    {

        [Required]
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }
    }

    public class CategoriaViewInaModel
    {
    }

    public class CategoriaListModel
    {
        [Required]
        [Display(Name = "Categoria: ")]
        public int id { get; set; }
    }

    public class CategoriaEditModel
    {
        [Display(Name = "Nombre: ")]
        public string nombre { get; set; }

        [Display(Name = "Descripcion: ")]
        public string descripcion { get; set; }

        [Display(Name = "Padre: ")]
        public int idCatPadre { get; set; }

        [Display(Name = "Activo: ")]
        public int activo { get; set; }
    }
}