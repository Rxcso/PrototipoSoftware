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
        private inf245netsoft db = new inf245netsoft();

        public ActionResult Index(string evento)
        {
            if (evento != "" && evento !=null)
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
        public ActionResult RegisterPromocion(PromocionModel model)
        {
            
            Promociones promocion = new Promociones();
            Promociones promocionL = db.Promociones.ToList().Last();
            promocion.codPromo = promocionL.codPromo + 1;
            if (Session["idEvento"] != null)
            {
                int ev =(int)Session["idEvento"];
                if (ev == 0) return RedirectToAction("Index", "Evento");
                promocion.codEvento = (int)Session["idEvento"];
            }
            else return View("Index");
            promocion.estado = true;

            if (model.codBanco != 0) //promocion por tarjeta
            {
                if (model.fechaIni < model.fechaFin & model.descuento>0)
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
                }
                
            }
            else //En caso de promocion por entradas
            {
                if (model.fechaIni < model.fechaFin & model.cantAdq >model.cantComp & model.cantComp>0 & model.cantAdq>0)
                {
                    promocion.fechaIni = model.fechaIni;
                    promocion.fechaFin = model.fechaFin;
                    promocion.modoPago = "E";
                    promocion.cantAdq = model.cantAdq;
                    promocion.cantComp = model.cantComp;
                    promocion.descripcion = model.cantAdq + "X" + model.cantComp;
                    db.Promociones.Add(promocion);
                    db.SaveChanges();
                }
            }
            return View("Index");
            //throw new Exception("Test Exception");
        }

        public ActionResult Delete2(int id, int ide)
        {
            Promociones prom = db.Promociones.Find(id, ide);
            db.Promociones.Remove(prom);
            //db.Entry(prom).State = EntityState.Modified;
            //prom.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Delete(string evento, string promocion)
        {
            if (promocion == "" || promocion == null) return View("Index");
            int idQ = int.Parse(evento);
            int ideQ = int.Parse(promocion);
            //ViewBag.idEvento = idQ;
            Promociones prom = db.Promociones.Find(ideQ, idQ);
            db.Promociones.Remove(prom);
            //db.Entry(prom).State = EntityState.Modified;
            //prom.estado = false;
            db.SaveChanges();
            //return RedirectToAction("Index", "Promocion");
            return View("Index");
        }

    }
}