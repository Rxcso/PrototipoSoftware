using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ReporteEventosModel
    {
        public int codigoEvento { get; set; }
        public string nombreEvento { get; set; }
        public string nombreOrganizador { get; set; }
        public DateTime fechaFuncion { get; set; }
        public DateTime horaFuncion { get; set; }
        public int entradasDisponibles { get; set; }
        public int entradasVendidas { get; set; }
        public string local { get; set; }
        public string region { get; set; }
    }
}
