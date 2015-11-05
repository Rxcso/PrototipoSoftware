using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class PaqueteEntradas
    {
        [Required(ErrorMessage = "No existe el evento seleccionado")]
        public int idEvento { get; set; }

        [Required(ErrorMessage = "No hay funcion seleccionada")]
        public int idFuncion { get; set; }

        [Required(ErrorMessage = "No hay zona seleccionada")]
        public int idZona { get; set; }

        public List<int> filas { get; set; }
        public List<int> columnas { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public bool tieneAsientos { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Se han elegido 0 entradas!")]
        public int cantEntradas { get; set; }

        public PaqueteEntradas()
        {

        }
        public PaqueteEntradas(int id)
        {
            idEvento = id;
            idFuncion=0;
            idZona=0;
            cantEntradas = 0;
            tieneAsientos = false;
        }

    }
}