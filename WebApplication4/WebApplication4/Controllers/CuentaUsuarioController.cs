using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class CuentaUsuarioController : Controller
    {
        private inf245netsoft db = new inf245netsoft();

        // GET: /CuentaUsuario/
        public ActionResult Index()
        {
            return View(db.CuentaUsuario.ToList());
        }

        // GET: /CuentaUsuario/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            if (cuentausuario == null)
            {
                return HttpNotFound();
            }
            return View(cuentausuario);
        }

        // GET: /CuentaUsuario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /CuentaUsuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="usuario,tipoUsuario,correo,contrasena,estado,tipoDoc,codDoc,nombre,apellido,direccion,telefono,telMovil,sexo,fechaNac,puntos,codPerfil")] CuentaUsuario cuentausuario)
        {
            if (ModelState.IsValid)
            {
                db.CuentaUsuario.Add(cuentausuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cuentausuario);
        }

        // GET: /CuentaUsuario/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            if (cuentausuario == null)
            {
                return HttpNotFound();
            }
            return View(cuentausuario);
        }

        // POST: /CuentaUsuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="usuario,tipoUsuario,correo,contrasena,estado,tipoDoc,codDoc,nombre,apellido,direccion,telefono,telMovil,sexo,fechaNac,puntos,codPerfil")] CuentaUsuario cuentausuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentausuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuentausuario);
        }

        // GET: /CuentaUsuario/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            if (cuentausuario == null)
            {
                return HttpNotFound();
            }
            return View(cuentausuario);
        }

        // POST: /CuentaUsuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            db.CuentaUsuario.Remove(cuentausuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
