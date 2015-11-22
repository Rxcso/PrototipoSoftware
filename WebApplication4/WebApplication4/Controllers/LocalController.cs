using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class LocalController : Controller
    {

        private inf245netsoft db = new inf245netsoft();
        // GET: Local
        public ActionResult Index()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View();
        }

        [HttpGet]
        public ActionResult RegisterLocal()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterLocal(LocalModel model)
        {
            if (ModelState.IsValid)
            {
                Local local = new Local();
                //Local localL = db.Local.ToList().Last();
                //local.codLocal = localL.codLocal + 1;
                local.descripcion = model.descripcion;
                local.ubicacion = model.ubicacion;
                local.estaActivo = true;
                local.idProvincia = model.idProv;
                local.idRegion = model.idRegion;
                db.Local.Add(local);
                db.SaveChanges();
                return RedirectToAction("Index", "Local");
            }
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            return View("Index");
        }

        public JsonResult Delete(string local)
        {
            int id = int.Parse(local);
            Local localr = db.Local.Find(id);
            //db.Local.Remove(localr);
            db.Entry(localr).State = EntityState.Modified;
            localr.estaActivo = false;
            db.SaveChanges();
            return Json("Local Desactivado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Active(string local)
        {
            //List<WebApplication4.Models.Local> listaLocal;
            int id = int.Parse(local);
            Local localr = db.Local.Find(id);
            //db.Local.Remove(localr);
            db.Entry(localr).State = EntityState.Modified;
            localr.estaActivo = true;
            db.SaveChanges();
            //listaLocal = db.Local.AsNoTracking().Where(c => c.estaActivo == true).ToList();
            Session["ListaL"] = null;
            return Json("Local Activado", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete2(int id)
        {
            Local local = db.Local.Find(id);
            db.Local.Remove(local);
            db.Entry(local).State = EntityState.Modified;
            local.estaActivo = false;
            db.SaveChanges();
            return RedirectToAction("Index", "Local");
        }

        public ActionResult Edit(string local)
        {
            if (local != null)
            {
                int id = int.Parse(local);
                ViewBag.id = id;
                TempData["codigol"] = id;
                Local lo = db.Local.Find(id);
                LocalModel lo1 = new LocalModel();
                Session["local"] = lo;
                int idl = (int)lo.idRegion;
                lo1.idRegion = (int)lo.idRegion;
                lo1.idProv = (int)lo.idProvincia;
                List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
                List<Region> listProv = db.Region.Where(c => c.idRegPadre == idl).ToList();
                ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre", lo1.idRegion);
                ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre", lo1.idProv);
            }
            return View("Edit");
        }

        public ActionResult Edit2(int id)
        {
            ViewBag.id = id;
            TempData["codigol"] = id;
            return View("Edit");
        }

        [HttpGet]
        public ActionResult EditRegister()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(LocalEditModel model)
        {
            if (ModelState.IsValid)
            {
                var o = ViewBag.id;
                Local local = db.Local.Find(TempData["codigol"]);
                db.Entry(local).State = EntityState.Modified;
                //local.aforo = model.aforo;
                local.descripcion = model.descripcion;
                local.ubicacion = model.ubicacion;
                local.idProvincia = model.idProv;
                local.idRegion = model.idRegion;
                db.SaveChanges();
                return RedirectToAction("Index", "Local");
            }
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(LocalSearchModel local)
        {
            List<Local> listaLoc = null;
            if (local.departamento == 0 && local.nombre != null) listaLoc = db.Local.AsNoTracking().Where(c => c.descripcion.Contains(local.nombre)).ToList();
            else if (local.departamento != 0 && local.nombre == null) listaLoc = db.Local.AsNoTracking().Where(c => local.departamento == c.idRegion).ToList();
            else if (local.departamento != 0 && local.nombre != null) listaLoc = db.Local.AsNoTracking().Where(c => c.descripcion.Contains(local.nombre) && local.departamento == c.idRegion).ToList();
            else TempData["ListaL"] = null;
            if (listaLoc != null) TempData["ListaL"] = listaLoc;
            else TempData["ListaL"] = null;
            return RedirectToAction("Index", "Local");
        }

        public ActionResult Search2(string local)
        {
            List<Local> listaLoc;
            if (local == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaL"] = null;
                return RedirectToAction("Index", "Local");
            }
            listaLoc = db.Local.AsNoTracking().Where(c => c.descripcion.Contains(local) && c.estaActivo == true).ToList();
            if (listaLoc != null) Session["ListaL"] = listaLoc;
            else Session["ListaL"] = null;
            return RedirectToAction("Index", "Local");
        }

        public ActionResult SearchI()
        {
            List<Local> listaLoc;
            listaLoc = db.Local.AsNoTracking().Where(c => c.estaActivo == false).ToList();
            if (listaLoc != null) Session["ListaL"] = listaLoc;
            else Session["ListaL"] = null;
            return RedirectToAction("Index", "Local");
        }

        public ActionResult Search3(string region)
        {
            int id = int.Parse(region);
            List<Local> listaLoc;
            if (region == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaL"] = null;
                return RedirectToAction("Index", "Local");
            }
            if (region == "0")
            {
                Session["ListaL"] = db.Local.Where(c => c.estaActivo == true).ToList();
                return RedirectToAction("Index", "Local");
            }
            listaLoc = db.Local.AsNoTracking().Where(c => c.idRegion == id && c.estaActivo == true).ToList();
            if (listaLoc != null) Session["ListaL"] = listaLoc;
            else Session["ListaL"] = null;
            return RedirectToAction("Index", "Local");
        }

    }
}