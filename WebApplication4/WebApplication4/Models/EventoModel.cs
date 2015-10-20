﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class EventoModel
    {
        [Required]
        [Display(Name="Nombre")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Descripcion")]
        public string descripcion { get; set; }

        public DateTime fecha_inicio { get; set; }

        public DateTime fecha_fin { get; set; }

        //public string estado { get; set; }

        //public Nullable<double> monto_transferir { get; set; }

        //public Nullable<double> monto_adeudado { get; set; }

        public string lugar { get; set; }

        //public Nullable<double> penalidadXcancelacion { get; set; }

        //public Nullable<double> penalidadXpostergacion { get; set; }

        //public Nullable<double> porccomision { get; set; }

        //public Nullable<bool> esUnico { get; set; }

        //public Nullable<int> idCategoria { get; set; }

        //public Nullable<int> idRegion { get; set; }

        //public Nullable<int> idPromotor { get; set; }

        //public Nullable<int> idOrganizador { get; set; }

    }
}