using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class PromocionController : Controller
    {
        // GET: Promocion
        private inf245netsoft db = new inf245netsoft();

        public ActionResult Index()
        {
            return View();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
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
            if ((model.descuento==0)&&(model.cantAdq==0))
            {
                return RedirectToAction("Index", "Promocion");
            }
            Promociones promocion = new Promociones();
            Promociones promocionL = db.Promociones.ToList().Last();
            promocion.codPromo = promocionL.codPromo + 1;
            promocion.codEvento = 1;
            promocion.estado = true;
            if (model.codBanco != 0)
            {
                promocion.codBanco = model.codBanco;
                promocion.codTipoTarjeta = model.codTipoTarjeta;
                promocion.fechaIni = model.fechaIni;
                promocion.fechaFin = model.fechaFin;
                promocion.descuento = model.descuento;
                promocion.modoPago = "T";
                promocion.descripcion = db.Banco.Find(model.codBanco).nombre + " " + db.TipoTarjeta.Find(model.codTipoTarjeta).nombre + " " + model.descuento+"%";
                db.Promociones.Add(promocion);
                db.SaveChanges();
                return RedirectToAction("Index", "Promocion");
            }
            else
            {
                promocion.fechaIni = model.fechaIni;
                promocion.fechaFin = model.fechaFin;
                promocion.modoPago = "E";
                promocion.cantAdq = model.cantAdq;
                promocion.cantComp = model.cantComp;
                promocion.descripcion = model.cantAdq + "X" + model.cantComp;
                db.Promociones.Add(promocion);
                db.SaveChanges();
                return RedirectToAction("Index", "Promocion");
            }
            //throw new Exception("Test Exception");
            
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterPromocion2(PromocionModel2 model)
        {

            Promociones promocion = new Promociones();
            Promociones promocionL = db.Promociones.ToList().Last();
            promocion.codPromo = promocionL.codPromo + 1;
            promocion.codEvento = 1;
            promocion.estado = true;            
            promocion.fechaIni = model.fechaIni;
            promocion.fechaFin = model.fechaFin;
            promocion.modoPago = "E";
            promocion.cantAdq = model.cantAdq;
            promocion.cantComp = model.cantComp;
            promocion.descripcion = model.cantAdq+ "X" +model.cantComp;
            //throw new Exception("Test Exception");
            db.Promociones.Add(promocion);
            db.SaveChanges();

            return RedirectToAction("Index", "Promocion");
        }

        public ActionResult Delete(int id, int ide)
        {
            Promociones prom = db.Promociones.Find(id, ide);
            db.Promociones.Remove(prom);
            
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

    }
}