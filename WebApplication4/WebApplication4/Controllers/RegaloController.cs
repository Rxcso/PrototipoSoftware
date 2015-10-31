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
                return View("Index");
            }
            return View("Index");
        }

        public ActionResult Delete2(int id)
        {
            Regalo regalo = db.Regalo.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(regalo).State = EntityState.Modified;
            regalo.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Delete(string regalo)
        {
            //if (regalo == "" || regalo == null) return View("Index");
            int idQ = int.Parse(regalo);
            Regalo regaloE = db.Regalo.Find(idQ);
            //db.Regalo.Remove(regalo);
            db.Entry(regaloE).State = EntityState.Modified;
            regaloE.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Edit(string regalo)
        {
            int id = int.Parse(regalo);
            ViewBag.id = id;
            TempData["codigo"] = id;
            Session["regalo"] = db.Regalo.Find(id);
            return View("Edit");
        }

        public ActionResult Edit2(int id)
        {
            ViewBag.id = id;
            TempData["codigo"] = id;
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(RegaloModel model)
        {
            if (ModelState.IsValid)
            {
                var o = ViewBag.id;
                Regalo regalo = db.Regalo.Find(TempData["codigo"]);
                db.Entry(regalo).State = EntityState.Modified;
                regalo.Nombre = model.nombre;
                regalo.descripcion = model.descripcion;
                regalo.puntos = model.puntos;
                db.SaveChanges();
                return RedirectToAction("Index", "Regalo");
            }
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(RegaloSearchModel regalo)
        {
            if (ModelState.IsValid)
            {
                List<Regalo> listaReg = db.Regalo.AsNoTracking().Where(c => c.Nombre.Contains(regalo.nombre)).ToList();
                if (listaReg != null) TempData["ListaR"] = listaReg;
                else TempData["ListaR"] = null;
                return RedirectToAction("Index", "Regalo");
            }
            TempData["ListaR"] = null;
            return RedirectToAction("Index", "Regalo");
        }

        public ActionResult Search2(string regalo)
        {
            List<Regalo> listaReg;
            if (regalo == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaR"] = null;
                return RedirectToAction("Index", "Regalo");
            }
            listaReg = db.Regalo.AsNoTracking().Where(c => c.Nombre.Contains(regalo) && c.estado == true).ToList();
            if (listaReg != null) Session["ListaR"] = listaReg;
            else Session["ListaR"] = null;
            return RedirectToAction("Index", "Regalo");
        }
    }
}