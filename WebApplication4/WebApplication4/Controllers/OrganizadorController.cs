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
                Organizador orgL = db.Organizador.ToList().Last();
                org.codOrg = orgL.codOrg + 1;
                org.codDoc = model.codDoc;
                org.correo = model.Email;
                org.nombOrg = model.nombre;
                org.telefOrg = model.telefono;
                org.tipoDoc = model.tipoDoc;
                org.estadoOrg = "Activo";
                db.Organizador.Add(org);
                db.SaveChanges();
                return RedirectToAction("Index", "Organizador");
            }
            return RedirectToAction("Index", "Organizador");
        }

        public ActionResult Delete(int id)
        {
            Organizador org = db.Organizador.Find(id);
            //db.Regalo.Remove(regalo);
            db.Entry(org).State = EntityState.Modified;
            org.estadoOrg = "Inactivo";
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Edit(int id)
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
            return RedirectToAction("Index", "Organizador");
        }

    }
}