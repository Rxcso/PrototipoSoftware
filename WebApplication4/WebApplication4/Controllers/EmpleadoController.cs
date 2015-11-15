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


        public JsonResult Delete(string usuario)
        {
            //CuentaUsuario cuenta = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc == usuario).ToList().First();
            //db.Regalo.Remove(regalo);
            string usuario2 = usuario.Replace("°", "@");
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario2);
            if (cuenta.codPerfil == 2)
            {
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
                List<Turno> ltu = db.Turno.Where(c => c.codTurnoSis == ts.codTurnoSis && c.usuario == cuenta.usuario && c.fecha == hoy && c.estadoCaja == "Abierto").ToList();
                if (ltu.Count != null && ltu.Count == 0)
                {
                    db.Entry(cuenta).State = EntityState.Modified;
                    cuenta.estado = false;
                    //db.SaveChanges();
                    List<Turno> ltur = db.Turno.Where(c => c.usuario == cuenta.usuario && c.fecha > hoy).ToList();
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
                    return Json("Error este vendedor esta trabajando en un punto de venta ahora", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                db.Entry(cuenta).State = EntityState.Modified;
                cuenta.estado = false;
                db.SaveChanges();
                return Json("Promotor desactivado", JsonRequestBehavior.AllowGet);
            }
            return Json("Empleado desactivado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Active(string usuario)
        {
            string usuario2 = usuario.Replace("°", "@");
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario2);
            if (cuenta.codPerfil == 2)
            {
                db.Entry(cuenta).State = EntityState.Modified;
                cuenta.estado = true;
                db.SaveChanges();
                Session["ListaV1"] = null;
            }
            else
            {
                db.Entry(cuenta).State = EntityState.Modified;
                cuenta.estado = true;
                db.SaveChanges();
                Session["ListaT"] = null;
                return Json("Promotor activado", JsonRequestBehavior.AllowGet);
            }
            return Json("Empleado activado", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string usuario)
        {
            string usuario2 = usuario.Replace("°", "@");
            ViewBag.id = usuario2;
            CuentaUsuario cuenta=db.CuentaUsuario.Find(usuario2);
            TempData["codigoE"] = usuario2;
            Session["usuarioE"] = cuenta;
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
                CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario);
                List<AspNetUsers> lcuentaAsp = db.AspNetUsers.Where(c => c.Email == cuenta.correo).ToList();
                AspNetUsers cuentaAsp = lcuentaAsp.First();
                //CuentaUsuario cuenta = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc == usuario).ToList().First();
                db.Entry(cuenta).State = EntityState.Modified;
                //cuenta.correo = model.Email;
                cuenta.direccion = model.direccion;
                cuenta.nombre = model.nombre;
                cuenta.telefono = model.telefono;
                cuenta.telMovil = model.telMovil;
                cuenta.apellido = model.apellido;
                db.SaveChanges();
                return RedirectToAction("Index", "Empleado");
            }
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search1(EmpleadoSearchModel vendedor)
        {
            if (ModelState.IsValid)
            {
                List<CuentaUsuario> listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.nombre.Contains(vendedor.nombre) && c.estado == true && c.codPerfil == 2).ToList();
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
                List<CuentaUsuario> listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.nombre.Contains(promotor.nombre) && c.estado == true && c.codPerfil == 3).ToList();
                if (listaEmp != null) TempData["ListaT"] = listaEmp;
                else TempData["ListaT"] = null;
                return RedirectToAction("Index", "Empleado");
            }
            TempData["ListaT"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        public ActionResult Search3(string nombre)
        {
            List<CuentaUsuario> listaEmp;
            if (nombre == "" || nombre ==null)
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaV1"] = null;
                return RedirectToAction("Index", "Empleado");
            }
            listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.nombre.Contains(nombre) && c.estado == true && c.codPerfil == 2).ToList();
            if (listaEmp != null) Session["ListaV1"] = listaEmp;
            else Session["ListaV1"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        public ActionResult SearchIE()
        {
            List<CuentaUsuario> listaEmp;
            listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.estado == false && c.codPerfil == 2).ToList();
            if (listaEmp != null) Session["ListaV1"] = listaEmp;
            else Session["ListaV1"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        public ActionResult SearchIP()
        {
            List<CuentaUsuario> listaEmp;
            listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.estado == false && c.codPerfil == 3).ToList();
            if (listaEmp != null) Session["ListaT"] = listaEmp;
            else Session["ListaT"] = null;
            return RedirectToAction("Index", "Empleado");
        }

        public ActionResult Search4(string nombre)
        {
            List<CuentaUsuario> listaEmp;
            if (nombre == "" || nombre ==null)
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaT"] = null;
                return RedirectToAction("Index", "Empleado");
            }
            listaEmp = db.CuentaUsuario.AsNoTracking().Where(c => c.nombre.Contains(nombre) && c.estado == true && c.codPerfil == 3).ToList();
            if (listaEmp != null) Session["ListaT"] = listaEmp;
            else Session["ListaT"] = null;
            return RedirectToAction("Index", "Empleado");
        }
    }
}