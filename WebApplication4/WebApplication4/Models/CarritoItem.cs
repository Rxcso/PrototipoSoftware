using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class CarritoItem
    {
        public int idEvento { get; set; }
        public int idFuncion { get; set; }
        public int idZona { get; set; }
        public string nombreEvento { get; set; }
        public DateTime fecha { get; set; }
        public DateTime hora { get; set; }
        public string zona { get; set; }
        public double precio { get; set; }
        public int cantidad { get; set; }
        public List<int> filas { get; set; }
        public List<int> columnas { get; set; }
    }
}