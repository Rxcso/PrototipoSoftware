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
            CuentaUsuario cuenta = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc == usuario).ToList().First();
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
        public ActionResult EditRegister(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var o = ViewBag.id;
                string usuario = Convert.ToString(TempData["codigoE"]);
                CuentaUsuario cuenta = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc == usuario).ToList().First();
                db.Entry(cuenta).State = EntityState.Modified;
                cuenta.correo = model.Email;
                cuenta.nombre = model.nombre;
                cuenta.telefono = model.telefono;
                cuenta.telMovil = model.telMovil;
                cuenta.apellido = model.apellido;
                db.SaveChanges();
                return RedirectToAction("Index", "Empleado");
            }
            return RedirectToAction("Index", "Empleado");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search1(EmpleadoSearchModel vendedor)
        {
            if (ModelState.IsValid)
            {
                List<CuentaUsuario> listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.nombre.StartsWith(vendedor.nombre) && c.estado==true && c.codPerfil==2).ToList();
                if (listaEmp != null) TempData["ListaV1"] = listaEmp;
                else TempData["ListaV1"] = null;
                return RedirectToAction("Index", "Empleado");
            }
            TempData["ListaV1"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search2(EmpleadoSearchModel promotor)
        {
            if (ModelState.IsValid)
            {
                List<CuentaUsuario> listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.nombre.StartsWith(promotor.nombre) && c.estado == true &&c.codPerfil==3).ToList();
                if (listaEmp != null) TempData["ListaT"] = listaEmp;
                else TempData["ListaT"] = null;
                return RedirectToAction("Index", "Empleado");
            }
            TempData["ListaT"] = null;
            return RedirectToAction("Index", "Empleado");
        }
        
    }
}