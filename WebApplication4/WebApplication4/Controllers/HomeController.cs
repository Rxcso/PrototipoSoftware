using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        inf245netsoft db = new inf245netsoft();

        public ActionResult Index()
        {
            List<Eventos> listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null) && (c.ImagenDestacado != MagicHelpers.NuevoEvento)).ToList();
            ViewBag.ListaDestacados = listaDestacados;
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}