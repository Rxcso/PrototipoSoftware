using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class DetalleVentaBoleta
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? Cantidad { get; set; }
        public string Zona { get; set; }
        public double? Subtotal { get; set; }
        public double? Precio { get; set; }
        public double? Descuento { get; set; }

        public double? Total { get; set; }


    }
}