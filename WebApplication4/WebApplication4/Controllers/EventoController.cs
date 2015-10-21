using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class EventoController : Controller
    {

        private inf245netsoft db = new inf245netsoft();

        // GET: Evento
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        //[Authorize]
        public ActionResult Register()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Register(EventoModel model)
        {

            if (model.ImageEvento == null || model.ImageEvento.ContentLength == 0){
                ModelState.AddModelError("ImageEvento", "Falta Imagen Evento");
            }
            if ( model.EsDestacado &&( model.ImageDestacado == null || model.ImageDestacado.ContentLength == 0))
            {
                ModelState.AddModelError("ImageDestacado", "Falta Imagen de Evento Destacado");
            }
            if (ModelState.IsValid)
            {

                if (model.EsDestacado && model.ImageDestacado != null && model.ImageDestacado.ContentLength > 0)
                {
                    var uploadDir = "~/Images";
                    model.ImageDestacado.SaveAs(uploadDir+"ImagenDestacada1.jpg");
                    
                }


                var eventp = new EventosPrueba();
                eventp.nombre = model.nombre;
                eventp.destacado = model.EsDestacado;
                //eventp.urlDestacado = "ImagenDestacada1.jpg";
                eventp.codEvento = 4;

                db.EventosPrueba.Add(eventp);
                db.SaveChanges();

                return Redirect("~/Home/Index2");  
             
            }

            return View(model);
        }
    }
}