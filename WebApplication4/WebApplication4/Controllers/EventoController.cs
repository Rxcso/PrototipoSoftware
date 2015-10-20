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
        public ActionResult RegisterRegalo(EventoModel model)
        {
            if (ModelState.IsValid)
            {
                Regalo regalo = new Regalo();
                Regalo regaloL = db.Regalo.ToList().Last();
                regalo.idRegalo = regaloL.idRegalo + 1;
                regalo.Nombre = model.nombre;
                regalo.estado = true;
                regalo.descripcion = model.descripcion;
                regalo.puntos = model.puntos;
                db.Regalo.Add(regalo);
                db.SaveChanges();
                return RedirectToAction("Index", "Regalo");
            }
            return RedirectToAction("Index", "Regalo");
        }
    }
}