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
        inf245netsoft db = new inf245netsoft();
        // GET: Evento
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre");
            return View();
        }

        public ActionResult Provincia()
        {
            return View();
        }
        [HttpGet]
        public JsonResult ListaProvincias(string depid = "")
        {
            List<Region> list = new List<Region>();
            int ID = 0;
            if (int.TryParse(depid, out ID))
            {
                list = db.Region.Where(c => c.idRegPadre == ID).OrderBy(c => c.nombre).ToList();
            }
            if (Request.IsAjaxRequest())
            {
                return new JsonResult
                {
                    Data = list,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                return new JsonResult
                {
                    Data = "No se encontraron provincias",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Register(EventoModel model)
        {

            //System.Console.WriteLine("gg");
            if (model.ImageEvento == null || model.ImageEvento.ContentLength == 0){
                ModelState.AddModelError("ImageEvento", "Se necesita la Imagen del Evento");
            }


            if (ModelState.IsValid)
            {
                var eventp = new Eventos();	
                if (model.ImageDestacado != null && model.ImageDestacado.ContentLength > 0)		
                {		
                    var uploadDir = "/Images/";
                    eventp.ImagenDestacado = uploadDir + "destacado" + model.ImageDestacado.FileName;
                    model.ImageDestacado.SaveAs(Server.MapPath("~/Images/" + "destacado" + model.ImageDestacado.FileName));                   
                }		
               	
               eventp.nombre = model.nombre;
               eventp.idOrganizador = 1;
               eventp.idRegion = 1;

               db.Eventos.Add(eventp);	
               db.SaveChanges();

                return Redirect("~/Home/Index2");               
            }

            return View(model);
        }
    }
}