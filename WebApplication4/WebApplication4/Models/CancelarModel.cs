using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApplication4.Models
{
    public class CancelarModel
    {

        public int idEvento { get; set; }

        public string nombreEvento { get; set; }

        public string organizador { get; set; }

        public int [] listIdFuncion { get; set; }

        public DateTime [] listDateFuncion { get; set; }

        public Boolean [] seCancela { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        public DateTime fechaRecojo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ser un valor positivo.")]
        public int diasRecojo { get; set; }

        [StringLength(200, ErrorMessage = "El limite de longitud es 200")]
        public string motivo { get; set; }
    }
}