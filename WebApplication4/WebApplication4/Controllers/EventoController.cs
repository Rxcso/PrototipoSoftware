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
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Register(EventoModel model)
        {

            //System.Console.WriteLine("gg");
            if (model.ImageEvento == null || model.ImageEvento.ContentLength == 0){
                ModelState.AddModelError("ImageEvento", "This field is required");
            }
            if ( model.EsDestacado &&( model.ImageEvento == null || model.ImageEvento.ContentLength == 0))
            {
                ModelState.AddModelError("ImageDestacado", "This field is required");
            }
            if (ModelState.IsValid)
            {


                return Redirect("~/Home/Index2");               
            }

            return View(model);
        }
    }
}