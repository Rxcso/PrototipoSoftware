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
            ViewBag.DepID = new SelectList(listaDep,"idRegion","nombre");
            ViewBag.ProvID = new SelectList(listProv,"idProv","nombre");
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Register(EventoModel model)
        {
            if (ModelState.IsValid)
            {
            }
            return RedirectToAction("Index", "Evento");
        }
    }
}