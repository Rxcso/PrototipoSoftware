using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class TarifaModel
    {
        public double Precio { get; set; }
    }
    public class ZonaEventoModel
    {
        public string Nombre { get; set; }
        public int Aforo { get; set; }
        public List<TarifaModel> ListaTarifas { get; set; }
    }
}