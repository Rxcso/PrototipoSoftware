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
    [Authorize]
    public class PromocionController : Controller
    {
        // GET: Promocion
        private static inf245netsoft db = new inf245netsoft();

        public static Promociones CalculaMejorPromocionTarjeta(int codEvento, int idBanco, int tipoTarjeta)
        {
            try
            {
                //busco las promociones que se encuentren activas
                List<Promociones> promociones = db.Promociones.Where(c => c.codEvento == codEvento && c.codBanco == idBanco && c.codTipoTarjeta == tipoTarjeta && c.estado == true && c.fechaIni <= DateTime.Today && DateTime.Today <= DateTime.Today).ToList();
                promociones.Sort((a, b) => ((double)a.descuento).CompareTo((double)b.descuento));
                return promociones.Last();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public ActionResult Index(string evento)
        {
            if (evento != "" && evento != null)
            {
                int id = int.Parse(evento);
                Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
                TempData["idEvento"] = int.Parse(evento);
                Session["idEvento"] = int.Parse(evento);
                Session["nombreEvento"] = queryEvento.nombre;
                TempData["nombreEvento"] = queryEvento.nombre;
                ViewBag.nombreEvento = queryEvento.nombre;
                ViewBag.idEvento = evento;
                return View();
            }
            return RedirectToAction("Index", "Evento");
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegisterPromocion()
        {
            return View("RegisterPromocion");
        }


        //
        // POST: /Promocion/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterPromocion(PromocionModel model)
        {
            int ev = 0;
            List<PeriodoVenta> listPer = new List<PeriodoVenta>();
            DateTime fechMin = DateTime.MaxValue;
            DateTime fechMax = DateTime.MinValue;
            Promociones promocion = new Promociones();
            //Promociones promocionL = db.Promociones.ToList().Last();
            //promocion.codPromo = promocionL.codPromo + 1;
            if (Session["idEvento"] != null)
            {
                ev = (int)Session["idEvento"];
                if (ev == 0) return RedirectToAction("Index", "Evento");
                promocion.codEvento = (int)Session["idEvento"];
                listPer = db.PeriodoVenta.AsNoTracking().Where(c => c.codEvento == ev).ToList();
                for (int i = 0; i < listPer.Count; i++)
                {
                    if (listPer[i].fechaInicio < fechMin) fechMin = (DateTime)listPer[i].fechaInicio;
                    if (listPer[i].fechaFin > fechMax) fechMax = (DateTime)listPer[i].fechaFin;
                }
                if (listPer.Count == 0)
                {

                    ViewBag.NoPeriodo = "No existe un periodo de venta para este evento";
                    return View("Index");
                }
            }
            else return View("Index");
            promocion.estado = true;

            if (model.fechaFin > fechMax || model.fechaIni < fechMin)
            {
                ViewBag.ErrorPeriodo = "Fechas deben estar dentro de un periodo de " + fechMin.ToString("dd/MM/yyyy") + " y " + fechMax.ToString("dd/MM/yyyy");
                return View("Index");
            }

            if (ModelState.IsValid) //promocion por tarjeta
            {
                promocion.codBanco = model.codBanco;
                promocion.codTipoTarjeta = model.codTipoTarjeta;
                promocion.fechaIni = model.fechaIni;
                promocion.fechaFin = model.fechaFin;
                promocion.descuento = model.descuento;
                promocion.modoPago = "T";
                promocion.descripcion = db.Banco.Find(model.codBanco).nombre + " " + db.TipoTarjeta.Find(model.codTipoTarjeta).nombre + " " + model.descuento + "%";
                db.Promociones.Add(promocion);
                db.SaveChanges();
                return Redirect("~/Promocion/Index?evento=" + ev);
            }
            return View("Index");
            //throw new Exception("Test Exception");
        }

        [HttpGet]
        public ActionResult RegisterPromocion2()
        {
            return View("RegisterPromocion2");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterPromocion2(PromocionModel2 model)
        {
            int ev = 0;
            List<PeriodoVenta> listPer = new List<PeriodoVenta>();
            DateTime fechMin = DateTime.MaxValue;
            DateTime fechMax = DateTime.MinValue;
            Promociones promocion = new Promociones();
            //Promociones promocionL = db.Promociones.ToList().Last();
            //promocion.codPromo = promocionL.codPromo + 1;
            if (Session["idEvento"] != null)
            {
                ev = (int)Session["idEvento"];
                if (ev == 0) return RedirectToAction("Index", "Evento");
                promocion.codEvento = (int)Session["idEvento"];
                listPer = db.PeriodoVenta.AsNoTracking().Where(c => c.codEvento == ev).ToList();
                for (int i = 0; i < listPer.Count; i++)
                {
                    if (listPer[i].fechaInicio < fechMin) fechMin = (DateTime)listPer[i].fechaInicio;
                    if (listPer[i].fechaFin > fechMax) fechMax = (DateTime)listPer[i].fechaFin;
                }
                if (listPer.Count == 0)
                {

                    ViewBag.NoPeriodo = "No existe un periodo de venta para este evento";
                    return View("Index");
                }
            }
            else return View("Index");
            promocion.estado = true;

            if (model.fechaFin > fechMax || model.fechaIni < fechMin)
            {
                ViewBag.ErrorPeriodo = "Fechas deben estar dentro de un periodo de " + fechMin.ToString("dd/MM/yyyy") + " y " + fechMax.ToString("dd/MM/yyyy");
                return View("Index");
            }

            if (ModelState.IsValid)
            {
                promocion.fechaIni = model.fechaIni;
                promocion.fechaFin = model.fechaFin;
                promocion.modoPago = "E";
                promocion.cantAdq = model.cantAdq;
                promocion.cantComp = model.cantComp;
                promocion.descripcion = model.cantAdq + "X" + model.cantComp;
                db.Promociones.Add(promocion);
                db.SaveChanges();
                return Redirect("~/Promocion/Index?evento=" + ev);
            }
            return View("Index");
            //throw new Exception("Test Exception");
        }

        public ActionResult Delete2(int id, int ide)
        {
            Promociones prom = db.Promociones.Find(id, ide);
            //db.Promociones.Remove(prom);
            db.Entry(prom).State = EntityState.Modified;
            prom.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public JsonResult Delete(string evento, string promocion)
        {
            if (promocion == "" || promocion == null) return Json("Error:debe seleccionar una promocion", JsonRequestBehavior.AllowGet);
            int idQ = int.Parse(evento);
            int ideQ = int.Parse(promocion);
            //ViewBag.idEvento = idQ;
            Promociones prom = db.Promociones.Find(ideQ, idQ);
            if (prom.estado == false) return Json("Error: La promocion seleccionada ya esta desactivada", JsonRequestBehavior.AllowGet);
            //db.Promociones.Remove(prom);
            db.Entry(prom).State = EntityState.Modified;
            prom.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Promocion");
            return Json("La Promocion ha sido descativada con exito", JsonRequestBehavior.AllowGet);
        }

    }
}