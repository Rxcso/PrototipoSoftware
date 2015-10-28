﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ExtrasModel
    {
        [Required(ErrorMessage="Este campo es obligatorio.")]
        [Display(Name = "Máximo de reservas")]
        [Range(0, int.MaxValue, ErrorMessage = "Inserte un valor positivo.")]
        public int MaxReservas { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Puntos que otorga al cliente")]
        [Range(0, int.MaxValue, ErrorMessage = "Debe ser un valor positivo.")]
        public int PuntosToCliente { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Ganancia sobre las ventas")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe ser un valor positivo.")]
        public double Ganancia { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Monto fijo por venta de entradas")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe ser un valor positivo.")]
        public double MontFijoVentEnt { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Penalidad por cancelacion")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe ser un valor positivo.")]
        public double PenCancelacion { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [Display(Name = "Penalidad por postergacion")]
        [Range(0, double.MaxValue, ErrorMessage = "Debe ser un valor positivo.")]
        public double PenPostergacion { get; set; }

        
        [Display(Name = "Imagen para destacado: ")]
        public string ImageDestacado { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio.")]
        [DataType(DataType.Upload)]
        [Display(Name = "Imagen para Evento: ")]
        public string ImageEvento { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Imagen para la distribución: ")]
        public string ImageSitios { get; set; }

        [Display(Name = "Permitir reservas vía web")]
        public bool PermitirReservasWeb { get; set; }

        [Display(Name = "Permitir boleto electrónico")]
        public bool PermitirBoletoElectronico { get; set; }

    }
}