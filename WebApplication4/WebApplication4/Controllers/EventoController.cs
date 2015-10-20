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
        // GET: Evento
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Register(EventoModel model)
        {
            if (ModelState.IsValid)
            {
            }
            return RedirectToAction("Index", "Regalo");
        }
    }
}