using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class LocalController : Controller
    {

        private inf245netsoft db = new inf245netsoft();
        // GET: Local
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterLocal(LocalModel model)
        {
            Local local = new Local();
            Local localL = db.Local.ToList().Last();
            local.codLocal = localL.codLocal + 1;
            local.descripcion = model.descripcion;
            local.aforo = model.aforo;
            local.ubicacion = model.ubicacion;
            local.idProvincia = model.provincia;
            local.idRegion = model.departamento;
            db.Local.Add(local);
            db.SaveChanges();
            return RedirectToAction("Index", "Local");
        }

        public ActionResult Delete(int id)
        {
            Local local = db.Local.Find(id);
            db.Local.Remove(local);
            //db.Entry(local).State = EntityState.Modified;
            //local.es
            db.SaveChanges();
            return View("Index");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.id = id;
            TempData["codigol"] = id;
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(LocalModel model)
        {
            var o = ViewBag.id;
            Local local = db.Local.Find(TempData["codigol"]);
            db.Entry(local).State = EntityState.Modified;
            local.aforo = model.aforo;
            local.descripcion = model.descripcion;
            local.ubicacion = local.ubicacion;
            local.idProvincia = model.provincia;
            local.idRegion = model.departamento;
            db.SaveChanges();
            return RedirectToAction("Index", "Local");
        }

    }
}