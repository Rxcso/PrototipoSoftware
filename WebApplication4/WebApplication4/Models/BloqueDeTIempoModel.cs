using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class BloqueDeTiempoModel
    {

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime fechaInicio { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime fechaFin { get; set; }
    }
}