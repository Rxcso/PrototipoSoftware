using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Boleta
    {

        public double? Total { get; set; }
        public List<DetalleVentaBoleta> detalles { get; set; }
    }
}