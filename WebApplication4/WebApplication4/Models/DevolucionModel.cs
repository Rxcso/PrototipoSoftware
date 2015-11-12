using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class DevolucionModel
    {
        public int codDev { get; set; }
        public int numDoc { get; set; }
        public string nombre { get; set; }
        public DateTime fecha { get; set; }
        public DateTime hora { get; set; }
        public string evento { get; set; }
        public int cantAsientos { get; set; }
        public double monto { get; set; }
        public string estado { get; set; }
        public string zona { get; set; }
    }
}