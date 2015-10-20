using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class PuntoVentaController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: PuntoVenta
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterPunto(PuntoVentaModel model)
        {
            if (ModelState.IsValid)
            {
                PuntoVenta punto = new PuntoVenta();
                PuntoVenta puntoL = db.PuntoVenta.ToList().Last();
                punto.codPuntoVenta = puntoL.codPuntoVenta + 1;
                punto.dirMAC = model.mac;
                punto.estaActivo = true;
                punto.ubicacion = model.ubicacion;
                db.PuntoVenta.Add(punto);
                db.SaveChanges();
                return RedirectToAction("Index", "PuntoVenta");
            }
            return RedirectToAction("Index", "PuntoVenta");
        }

        public ActionResult Delete(int id)
        {
            PuntoVenta punto = db.PuntoVenta.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(punto).State = EntityState.Modified;
            punto.estaActivo = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Edit(int id)
        {
            ViewBag.id = id;
            TempData["codigoP"] = id;
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(PuntoVentaModel model)
        {
            if (ModelState.IsValid)
            {
                var o = ViewBag.id;
                PuntoVenta punto = db.PuntoVenta.Find(TempData["codigoP"]);
                db.Entry(punto).State = EntityState.Modified;
                punto.dirMAC = model.mac;
                punto.ubicacion = model.ubicacion;
                db.SaveChanges();
                return RedirectToAction("Index", "PuntoVenta");
            }
            return RedirectToAction("Index", "PuntoVenta");
        }
    }
}