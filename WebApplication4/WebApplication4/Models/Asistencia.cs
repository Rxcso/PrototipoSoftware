using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    [Serializable]
    public class Asistencia
    {
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }
        public DateTime? Fecha { get; set; }
        public string Nombre { get; set; }
        public bool Asistio { get; set; }

    }
}