using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{

    public class DatosGeneralesModel
    {
        [Required]
        [Display(Name = "Nombre:")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Organizador:")]
        public int idOrganizador { get; set; }

        [Required]
        [Display(Name = "Tipo:")]
        public int idCategoria { get; set; }

        [Display(Name = "Categoría:")]
        public int idSubCat { get; set; }

        [Display(Name = "Local:")]
        public int idLocal { get; set; }

        [Display(Name = "Direccion")]
        public string Direccion { get; set; }

        [Display(Name = "Departamento:")]
        public int idRegion { get; set; }

        [Display(Name = "Provincia:")]
        public int idProv { get; set; }

        [Display(Name = "Descripcion:")]
        public string descripcion { get; set; }
    }
    public class BloqueDeTiempoModel{

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime fechaInicio { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime fechaFin { get; set; }
    }
    public class FuncionesModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime fechaFuncion { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime horaInicio { get; set; }
        
    }
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
        [Display(Name="Monto fijo por venta de entradas")]
        public double MontFijoVentEnt { get; set; }

        [Required]
        [Display(Name="Penalidad por cancelacion")]
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