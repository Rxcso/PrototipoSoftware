using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ExtrasModel
    {
        [Display(Name = "Máximo de reservas")]
        public int MaxReservas { get; set; }

        [Required]
        [Display(Name = "Puntos que otorga al cliente")]
        public int PuntosToCliente { get; set; }

        [Required]
        [Display(Name = "Ganancia sobre las ventas")]
        public double Ganancia { get; set; }

        [Required]
        [Display(Name = "Monto fijo por venta de entradas")]
        public double MontFijoVentEnt { get; set; }

        [Required]
        [Display(Name = "Penalidad por cancelacion")]
        public double PenCancelacion { get; set; }

        [Required]
        [Display(Name = "Penalidad por postergacion")]
        public double PenPostergacion { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Imagen para destacado: ")]
        public HttpPostedFileBase ImageDestacado { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Imagen para Evento: ")]
        public HttpPostedFileBase ImageEvento { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Imagen para la distribución: ")]
        public HttpPostedFileBase ImageSitios { get; set; }

        [Display(Name = "Permitir reservas vía web")]
        public bool PermitirReservasWeb { get; set; }

        [Display(Name = "Permitir boleto electrónico")]
        public bool PermitirBoletoElectronico { get; set; }

    }
}