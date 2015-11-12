using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class OrganizadorController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: Organizador
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterOrganizador(OrganizadorModel model)
        {
            if (ModelState.IsValid)
            {
                Organizador org = new Organizador();
                //Organizador orgL = db.Organizador.ToList().Last();
                //org.codOrg = orgL.codOrg + 1;
                org.codDoc = model.codDoc;
                org.correo = model.Email;
                org.nombOrg = model.nombre;
                org.telefOrg = model.telefono;
                org.tipoDoc = model.tipoDoc;
                org.estadoOrg = "Activo";
                db.Organizador.Add(org);
                db.SaveChanges();
                return View("Index");
            }
            return View("Index");
        }

        public JsonResult Delete(string organizador)
        {
            int id = int.Parse(organizador);
            Organizador org = db.Organizador.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(org).State = EntityState.Modified;
            org.estadoOrg = "Inactivo";
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return Json("Organizador Desactivado", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete2(int id)
        {
            Organizador org = db.Organizador.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(org).State = EntityState.Modified;
            org.estadoOrg = "Inactivo";
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Edit(string organizador)
        {
            int id = int.Parse(organizador);
            ViewBag.id = id;
            TempData["codigoO"] = id;
            Session["organizador"] = db.Organizador.Find(id);
            return View("Edit");
        }

        public ActionResult Edit2(int id)
        {
            ViewBag.id = id;
            TempData["codigoO"] = id;
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(OrganizadorModel model)
        {
            if (ModelState.IsValid)
            {
                var o = ViewBag.id;
                Organizador org = db.Organizador.Find(TempData["codigoO"]);
                db.Entry(org).State = EntityState.Modified;
                org.correo = model.Email;
                org.nombOrg = model.nombre;
                org.telefOrg = model.telefono;
                org.codDoc = model.codDoc;
                org.tipoDoc = model.tipoDoc;
                db.SaveChanges();
                return RedirectToAction("Index", "Organizador");
            }
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(OrganizadorSearchModel organizador)
        {
            if (ModelState.IsValid)
            {
                List<Organizador> listaOrg = db.Organizador.AsNoTracking().Where(c => c.nombOrg.Contains(organizador.nombre)).ToList();
                if (listaOrg != null) TempData["ListaO"] = listaOrg;
                else TempData["ListaO"] = null;
                return RedirectToAction("Index", "Organizador");
            }
            TempData["ListaO"] = null;
            return RedirectToAction("Index", "Organizador");
        }

        public ActionResult Search2(string organizador)
        {
            List<Organizador> listaOrg;
            if (organizador == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaO"] = null;
                return RedirectToAction("Index", "Organizador");
            }
            listaOrg = db.Organizador.AsNoTracking().Where(c => c.nombOrg.Contains(organizador) && c.estadoOrg == "Activo").ToList();
            if (listaOrg != null) Session["ListaO"] = listaOrg;
            else Session["ListaO"] = null;
            return RedirectToAction("Index", "Organizador");
        }

        public ActionResult SearchI()
        {
            List<Organizador> listaOrg;
            listaOrg = db.Organizador.AsNoTracking().Where(c => c.estadoOrg == "Inactivo").ToList();
            if (listaOrg != null) Session["ListaO"] = listaOrg;
            else Session["ListaO"] = null;
            return RedirectToAction("Index", "Organizador");
        }

    }
}