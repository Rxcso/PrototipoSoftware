using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ReporteModel
    {

        public class ReporteBuscaModel
        {
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha Inicio:")]
            public System.DateTime fechaI { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha Fin:")]
            public System.DateTime fechaF { get; set; }
        }

       

        public class ReporteVentasDiaModel
        {
            public int codigo { get; set; }

            public string nombre { get; set; }

            public int cant { get; set; }

            public double total { get; set; }
        }

        public class ReporteVentas1Model
        {
            public System.DateTime fecha { get; set; }

            //public string codigo { get; set; }

            public string cliente { get; set; } 

            public string nombre { get; set; }     

            public double devtotal { get; set; }

            public double total { get; set; }
        }

        public class ReporteVentas2Model
        {
            public string codigo { get; set; }
            public string nombre { get; set; }

            public double total { get; set; }
        }

        public class ReporteVentas3Model
        {
            public int codigo { get; set; }

            public string nombre { get; set; }

            public string organizador { get; set; }

            public string funcion { get; set; }

           public int cant { get; set; }

            public double total { get; set; }
        }

        public class ReporteVentas4Model
        {
            public int codigo { get; set; }

            public string nombre { get; set; }

            public string ubicacion { get; set; }

            public string distrito { get; set; }

            public string provincia { get; set; }

           public double total { get; set; }
        }
}
    }
