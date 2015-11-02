using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class PromocionModel 
    {
            [Required]
            [DataType(DataType.Date)]
            [CheckDateRangeAttribute(ErrorMessage = "Fecha Inicio debe ser mayor que hoy")]
            [Display(Name = "Fecha Inicio:")]
            public System.DateTime fechaIni { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [GreaterThan("fechaIni", ErrorMessage = "Fecha fin debe ser mayor que fecha inicio")]
            [Display(Name = "Fecha Fin:")]
            public System.DateTime fechaFin { get; set; }

            [Required]
            [Range(0, 100)]
            [Display(Name = "Porcentaje Descuento:")]
            public float descuento { get; set; }

            [Required]
            [Display(Name = "Banco:")]
            public int codBanco { get; set; }

            [Required]
            [Display(Name = "Tipo Tarjeta:")]
            public int codTipoTarjeta { get; set; }


            public class CheckDateRangeAttribute : ValidationAttribute
            {
                public override bool IsValid(object value)
                {
                    DateTime dt = (DateTime)value;
                    if (dt >= DateTime.Now)
                    {
                        return true;
                    }

                    return false;
                }

            }

    }

    public class PromocionModel2
    {
        [Required]
        [DataType(DataType.Date)]
        [CheckDateRangeAttribute(ErrorMessage = "Fecha Inicio debe ser mayor que hoy")]
        [Display(Name = "Fecha Inicio:")]
        public System.DateTime fechaIni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [GreaterThan("fechaIni", ErrorMessage = "Fecha fin debe ser mayor que fecha inicio")]
        [Display(Name = "Fecha Fin:")]
        public System.DateTime fechaFin { get; set; }

        [Required]
        [Range(0, 2147483647)]
        [GreaterThan("cantComp", ErrorMessage = "Cantidad Adquirida debe ser mayor que cantidad comprada")]
        [Display(Name = "Cantidad Adquirida:")]
        public int cantAdq { get; set; }

        [Required]
        [Range(0, 2147483647)]
        [Display(Name = "Cantidad Comprada:")]
        public int cantComp { get; set; }

        public class CheckDateRangeAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                DateTime dt = (DateTime)value;
                if (dt >= DateTime.Now)
                {
                    return true;
                }

                return false;
            }

        }

    }
    
}