using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class EventosPruebaController : Controller
    {
        public WebApplication4.Models.inf245netsoft bd = new WebApplication4.Models.inf245netsoft();



        // GET: EventosPrueba
        public ActionResult Index( )
        {


            try
            {


                ViewBag.Categorias = bd.Categoria.AsNoTracking().Where(c => c.nivel == 1);
                ViewBag.Departamentos = bd.Region.AsNoTracking().Where(c => c.idRegPadre == null);


            }
            catch {


            }

                               
                return View();
        }

    /*
       [HttpPost]
        public ActionResult Busqueda( FormCollection collection ) {

           
           
            string valor = collection["busqueda"];

            var lista = bd.EventosPrueba.AsNoTracking().Where(c => c.nombre.Contains(valor)).ToList();

            ViewBag.lista = lista;
            ViewBag.Categorias = bd.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.Departamentos = bd.Region.AsNoTracking().Where(c => c.idRegPadre == null);

            return View("Index");
        }*/


        [HttpPost]
        public ActionResult BusquedaAvanzada(FormCollection collection)
        {

            int idRegion = int.Parse( collection["departamentos"]);

            var lista = bd.Region.AsNoTracking().Where(c => c.idRegPadre == idRegion).ToList();
            ViewBag.distritos = lista;

            ViewBag.Categorias = bd.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.Departamentos = bd.Region.AsNoTracking().Where(c => c.idRegPadre == null);


            return View("Index");
        }

       
        
        public ActionResult GetDistritosPorDepart(int id)
        {


  
            return View();
          


        }


    }
}