using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class TipoDeCambioController : Controller
    {

        private inf245netsoft db = new inf245netsoft();
        // GET: TipoDeCambio
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterTipoDeCambio(TipoDeCambioModel model)
        {
            List<WebApplication4.Models.TipoDeCambio> listaCambio = db.TipoDeCambio.AsNoTracking().Where(c => c.estado == "Activo").ToList();
            TipoDeCambio tipoA = listaCambio.First();
            db.Entry(tipoA).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                TipoDeCambio tipo = new TipoDeCambio();
                tipo.valor = model.valor;
                tipo.fecha = DateTime.Now;
                tipo.estado = "Activo";
                tipoA.estado = "Inactivo";
                db.TipoDeCambio.Add(tipo);
                db.SaveChanges();
                return RedirectToAction("Index", "TipoDeCambio");
            }
            return RedirectToAction("Index", "TipoDeCambio");
        }



    }
}