using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class VenderEntradaModel
    {
        [Required(ErrorMessage = "Debe ingresar un nombre.")]
        [Display(Name = "Nombre:")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚ ]*$", ErrorMessage = "Nombre no debe ser alfanumerico.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar un DNI o Pasaporte.")]
        [Display(Name = "DNI/Pasaporte:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Dni debe ser numérico.")]
        public string Dni { get; set; }

        [Display(Name = "Banco:")]
        public int? idBanco { get; set; }

        [Display(Name = "Tipo de Tarjeta:")]
        public int? idTipoTarjeta { get; set; }

        [Display(Name = "Nro. de Tarjeta:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El numero de tarjeta debe ser numerico.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Ingrese un numero de tarjeta valido.")]
        public string NumeroTarjeta { get; set; }

        [Display(Name = "CCV:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El codigo CCV debe ser numerico.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Ingrese un codigo CCV valido.")]
        public string CodCcv { get; set; }

        [Required(ErrorMessage = "Ingrese mes de vencimiento de la tarjeta.")]
        [Display(Name = "Mes:")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "Ingrese año de vencimiento de la tarjeta.")]
        [Display(Name = "Año:")]
        public int AnioVen { get; set; }

        [Display(Name = "Importe:")]
        public double Importe { get; set; }

        [Display(Name = "Descuento:")]
        public double Descuento { get; set; }

        [Required(ErrorMessage = "Debe ingresar un monto a pagar en efectivo")]
        [Display(Name = "Monto en Efectivo:")]
        public double MontoEfe { get; set; }

        [Display(Name = "Monto en Tarjeta:")]
        public double MontoTar { get; set; }

        [Display(Name = "Monto a pagar:")]
        public double MontoPagar { get; set; }

        [Display(Name = "Vuelto:")]
        public double Vuelto { get; set; }
        public List<int> idEventos { get; set; }
        public List<int> idPromociones { get; set; }
        public int idVenta { get; set; }
    }
}