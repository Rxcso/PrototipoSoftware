using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class FuncionesModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha:")]
        public DateTime fechaFuncion { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Hora:")]
        public DateTime horaInicio { get; set; }

    }
}