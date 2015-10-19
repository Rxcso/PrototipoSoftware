using System;
using System.Collections.Generic;
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
            local.idProvincia = 1;
            local.idRegion = 1;
            db.Local.Add(local);
            db.SaveChanges();
            return RedirectToAction("Index", "Local");
        }

        public ActionResult Delete(int id)
        {
            Local local = db.Local.Find(id);
            db.Local.Remove(local);

            db.SaveChanges();
            return View("Index");
        }

    }
}