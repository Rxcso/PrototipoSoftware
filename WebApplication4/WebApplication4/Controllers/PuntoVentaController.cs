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
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View();
        }

        [HttpGet]
        public ActionResult RegisterPunto()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
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
                punto.idProvincia = model.idProv;
                punto.idRegion = model.idRegion;
                db.PuntoVenta.Add(punto);
                db.SaveChanges();
                return RedirectToAction("Index", "PuntoVenta");
            }
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View("Index");
        }

        //public ActionResult Delete(string punto)
        //{
        //    int id = int.Parse(punto);
        //    PuntoVenta punto2 = db.PuntoVenta.Find(id);
        //    //db.Regalo.Remove(regalo);
        //    db.Entry(punto2).State = EntityState.Modified;
        //    punto2.estaActivo = false;
        //    db.SaveChanges();
        //    //return RedirectToAction("Index", "Evento");
        //    return RedirectToAction("Index", "PuntoVenta");
        //}

        public JsonResult Delete(string punto)
        {
            int id = int.Parse(punto);
            PuntoVenta punto2 = db.PuntoVenta.Find(id);
            //db.Regalo.Remove(regalo);
            DateTime hoy = DateTime.Now.Date;
            TurnoSistema ts = new TurnoSistema();
            TimeSpan da = DateTime.Now.TimeOfDay;
            List<TurnoSistema> listaturno = db.TurnoSistema.AsNoTracking().Where(c => c.activo == true).ToList();
            for (int i = 0; i < listaturno.Count; i++)
            {
                if ((TimeSpan.Parse(listaturno[i].horIni) < da) && (TimeSpan.Parse(listaturno[i].horFin) > da))
                {
                    ts = listaturno[i];
                }
            }
            List<Turno> ltu = db.Turno.Where(c => c.codTurnoSis == ts.codTurnoSis && c.codPuntoVenta == id && c.fecha == hoy && c.estadoCaja == "Abierto").ToList();
            if (ltu.Count != null && ltu.Count == 0)
            {
                db.Entry(punto2).State = EntityState.Modified;
                punto2.estaActivo = false;
                //db.SaveChanges();
                List<Turno> ltur = db.Turno.Where(c => c.codPuntoVenta == id).ToList();
                for (int j = 0; j < ltur.Count; j++)
                {
                    db.Turno.Remove(ltur[j]);
                }
                db.SaveChanges();
            }
            else
            {
                Session["vendAsig"] = null;
                Session["ListaTurnoVendedor"] = null;
                return Json("Error este punto de venta esta en uso en este momento", JsonRequestBehavior.AllowGet);
            }
            //return RedirectToAction("Index", "Evento");
            return Json("Punto de Venta desactivado", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete2(int id)
        {
            PuntoVenta punto = db.PuntoVenta.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(punto).State = EntityState.Modified;
            punto.estaActivo = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return RedirectToAction("Index", "PuntoVenta");
        }

        public ActionResult Edit(string punto)
        {
            if (punto != null)
            {
                int id = int.Parse(punto);
                ViewBag.id = id;
                TempData["codigoP"] = id;
                PuntoVenta pu = db.PuntoVenta.Find(id);
                Session["punto"] = pu;
                PuntoVentaModel ptm = new PuntoVentaModel();
                int idl = (int)pu.idRegion;
                ptm.idRegion = (int)pu.idRegion;
                ptm.idProv = (int)pu.idProvincia;
                List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
                List<Region> listProv = db.Region.Where(c => c.idRegPadre == idl).ToList();
                ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre", ptm.idRegion);
                ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre", ptm.idProv);


            }
            return View("Edit");
        }

        public ActionResult Edit2(int id)
        {
            ViewBag.id = id;
            TempData["codigoP"] = id;
            return View("Edit");
        }

        [HttpGet]
        public ActionResult EditRegister()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View();
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
                punto.idRegion = model.idRegion;
                punto.idProvincia = model.idProv;
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
                List<PuntoVenta> listaP = db.PuntoVenta.AsNoTracking().Where(c => c.ubicacion.Contains(punto.ubicacion)).ToList();
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
            listaP = db.PuntoVenta.AsNoTracking().Where(c => c.ubicacion.Contains(punto) && c.estaActivo == true).ToList();
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