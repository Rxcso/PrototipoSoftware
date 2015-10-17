using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class PromocionModel
    {
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha Inicio:")]
            public System.DateTime fechaIni { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha Fin:")]
            public System.DateTime fechaFin { get; set; }

            [Required]
            [Range(0, 100)]
            [Display(Name = "Porcentaje Descuento:")]
            public float descuento { get; set; }

            [Required]
            [Display(Name = "Descripcion")]
            public string descripcion { get; set; }

            [Required]
            [Display(Name = "Cantidad Adquirida")]
            public int cantAdq { get; set; }

            [Required]
            [Display(Name = "Cantidad Comprada")]
            public int cantComp { get; set; }

            [Required]
            [Display(Name = "Banco")]
            public int codBanco { get; set; }

            [Required]
            [Display(Name = "Tipo Tarjeta: ")]
            public int codTipoTarjeta { get; set; }
    }

    public class PromocionModel2
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio:")]
        public System.DateTime fechaIni { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin:")]
        public System.DateTime fechaFin { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Porcentaje Descuento:")]
        public float descuento { get; set; }

        [Required]
        [Display(Name = "Descripcion")]
        public string descripcion { get; set; }

        [Required]
        [Display(Name = "Cantidad Adquirida")]
        public int cantAdq { get; set; }

        [Required]
        [Display(Name = "Cantidad Comprada")]
        public int cantComp { get; set; }

        [Required]
        [Display(Name = "Banco")]
        public int codBanco { get; set; }

        [Required]
        [Display(Name = "Tipo Tarjeta: ")]
        public int codTipoTarjeta { get; set; }
    }

}