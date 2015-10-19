using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class RegaloController : Controller
    {

        private inf245netsoft db = new inf245netsoft();
        // GET: Regalo
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Regalo/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterRegalo(RegaloModel model)
        {
            Regalo regalo = new Regalo();
            Regalo regaloL = db.Regalo.ToList().Last();
            regalo.idRegalo = regaloL.idRegalo + 1;
            regalo.Nombre = model.nombre;
            regalo.descripcion = model.descripcion;
            regalo.puntos = model.puntos;
            db.Regalo.Add(regalo);
            db.SaveChanges();
            return RedirectToAction("Index", "Regalo");
        }

        public ActionResult Delete(int id)
        {
            Regalo regalo = db.Regalo.Find(id);
            db.Regalo.Remove(regalo);
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        [HttpPost]
        public ActionResult Edit(Regalo regalo)
        {
            db.Regalo.Attach(regalo);
            db.SaveChanges();
            return View("Index");
        }
    }
}