using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class Evento2Model
    {

        public int id { get; set; }

        [Required]
        [Display(AutoGenerateField = false)]
        public string nombre { get; set; }

        //[Required]

        public DateTime fechaInicio { get; set; }

        public DateTime fechaFin { get; set; }


        public int idOrganizador { get; set; }

        //[Required]
        [Display(Name = "")]
        public int idCategoria { get; set; }

        //[Required]
        [Display(Name = "")]
        public int idSubCat { get; set; }

        //[Display(Name = "Local:")]
        public int idLocal { get; set; }

        //[Display(Name = "Dirección:")]
        public string lugar { get; set; }

        //[Required]
        [Display(Name = "")]
        public int idRegion { get; set; }

        //[Required]
        [Display(Name = "")]
        public int idProv { get; set; }

        //[Required]




        public int cantidadMaximaMostrar { get; set; }


        public int posicionActual { get; set; }


        public int numeroPaginas { get; set; }


        public string descripcion { get; set; }
        //TERMINA DATOS GENERALES
        //public Nullable<double> penalidadXcancelacion { get; set; }

        //public Nullable<double> penalidadXpostergacion { get; set; }

        //public Nullable<double> porccomision { get; set; }

        //public Nullable<bool> esUnico { get; set; }

        //public Nullable<int> idPromotor { get; set; }
        [DataType(DataType.Upload)]

        public HttpPostedFileBase ImageDestacado { get; set; }

        [Required]
        [DataType(DataType.Upload)]

        public HttpPostedFileBase ImageEvento { get; set; }

        [DataType(DataType.Upload)]

        public HttpPostedFileBase ImageSitios { get; set; }



        public static EventoModel getEvento(Eventos evento)
        {
            var model = new EventoModel();
            model.nombre = evento.nombre;
            model.descripcion = evento.descripcion;

            return model;
        }


        public List<Eventos> ListaEventos { get; set; }


        public int cantidadEventos()
        {

            return ListaEventos.Count();

        }

        public Evento2Model(int maxContenido)
        {

            this.cantidadMaximaMostrar = maxContenido;
            this.posicionActual = 1;
            ListaEventos = new List<Eventos>();


        }
    }
}