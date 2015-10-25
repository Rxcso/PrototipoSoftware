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
                punto.idProvincia = model.provincia;
                punto.idRegion = model.departamento;
                db.PuntoVenta.Add(punto);
                db.SaveChanges();
                return View("Index");
            }
            return View("Index");
        }

        public ActionResult Delete(string punto)
        {
            int id = int.Parse(punto);
            PuntoVenta punto2 = db.PuntoVenta.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(punto2).State = EntityState.Modified;
            punto2.estaActivo = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Delete2(int id)
        {
            PuntoVenta punto = db.PuntoVenta.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(punto).State = EntityState.Modified;
            punto.estaActivo = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Edit(string punto)
        {
            int id = int.Parse(punto);
            ViewBag.id = id;
            TempData["codigoP"] = id;
            Session["punto"] = db.PuntoVenta.Find(id);
            return View("Edit");
        }

        public ActionResult Edit2(int id)
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
                punto.idRegion = model.departamento;
                punto.idProvincia = model.provincia;
                db.SaveChanges();
                return RedirectToAction("Index", "PuntoVenta");
            }
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(PuntoVentaSearchModel punto)
        {
            if (ModelState.IsValid)
            {
                List<PuntoVenta> listaP = db.PuntoVenta.AsNoTracking().Where(c => c.ubicacion.StartsWith(punto.ubicacion)).ToList();
                if (listaP != null) TempData["ListaP"] = listaP;
                else TempData["ListaP"] = null;
                return RedirectToAction("Index", "PuntoVenta");
            }
            TempData["ListaP"] = null;
            return RedirectToAction("Index", "PuntoVenta");
        }

        public ActionResult Search2(string punto)
        {
            List<PuntoVenta> listaP;
            if (punto == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaP"] = null;
                return RedirectToAction("Index", "PuntoVenta");
            }
            listaP = db.PuntoVenta.AsNoTracking().Where(c => c.ubicacion.StartsWith(punto) && c.estaActivo == true).ToList();
            if (listaP != null) Session["ListaP"] = listaP;
            else Session["ListaP"] = null;
            return RedirectToAction("Index", "PuntoVenta");
        }

        public ActionResult Search3(string region)
        {
            int id = int.Parse(region);
            List<PuntoVenta> listaPunto;
            if (region == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaP"] = null;
                return RedirectToAction("Index", "PuntoVenta");
            }
            if (region == "0")
            {
                Session["ListaP"] = db.PuntoVenta.AsNoTracking().Where(c => c.estaActivo == true).ToList();
                return RedirectToAction("Index", "PuntoVenta");
            }
            listaPunto = db.PuntoVenta.AsNoTracking().Where(c => c.idRegion == id && c.estaActivo == true).ToList();
            if (listaPunto != null) Session["ListaP"] = listaPunto;
            else Session["ListaP"] = null;
            return RedirectToAction("Index", "PuntoVenta");
        }

    }
}