using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Entrada
    {
        public string Codigo { get; set; }
        public string Evento { get; set; }
        public string Zona { get; set; }
        public string Direccion { get; set; }
        public string Asiento { get; set; }
        public string Lugar { get; set; }
        public double? Precio { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public string Local { get; set; }
        public int codEvento { get; set; }
        public string Hora { get; set; }
    }
}