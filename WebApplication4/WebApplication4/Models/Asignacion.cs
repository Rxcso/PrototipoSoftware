using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    [Serializable]
    public class Asignacion
    {

        public DateTime Dia { get; set; }
        public string PuntoVenta { get; set; }
        public string Nombre { get; set; }
        public string Horas { get; set; }
    }
}