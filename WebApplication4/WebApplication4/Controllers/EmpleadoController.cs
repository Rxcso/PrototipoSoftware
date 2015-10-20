using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class EmpleadoController : Controller
    {

        private inf245netsoft db = new inf245netsoft();

        // GET: Empleado
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Delete(string usuario)
        {
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario);
            //db.Regalo.Remove(regalo);
            db.Entry(cuenta).State = EntityState.Modified;
            cuenta.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Edit(string usuario)
        {
            ViewBag.id = usuario;
            TempData["codigoE"] = usuario;
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(RegisterViewModel model)
        {
            var o = ViewBag.id;
            CuentaUsuario cuenta = db.CuentaUsuario.Find(TempData["codigoE"]);
            db.Entry(cuenta).State = EntityState.Modified;
            cuenta.correo = model.Email;
            cuenta.nombre = model.nombre;
            cuenta.telefono = model.telefono;
            cuenta.codDoc = model.codDoc;
            cuenta.tipoDoc = model.tipoDoc;
            cuenta.sexo = model.sexo;
            cuenta.telMovil = model.telMovil;
            cuenta.apellido = model.apellido;
            db.SaveChanges();
            return RedirectToAction("Index", "Empleado");
        }
        
    }
}