using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers

{
    [Authorize(Roles="Administrador")]
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
            double b,d;
            List<WebApplication4.Models.TipoDeCambio> listaCambio = db.TipoDeCambio.AsNoTracking().Where(c => c.estado == "Activo").ToList();
            if (listaCambio.Count > 0)
            {
                TipoDeCambio tipoA = listaCambio.Last();
                db.Entry(tipoA).State = EntityState.Modified;
                if (ModelState.IsValid)
                {
                    TipoDeCambio tipo = new TipoDeCambio();
                    d = model.valor * 10000;
                    b = System.Math.Truncate(d);
                    int n = int.Parse(b + "");
                    tipo.valor = n;
                    tipo.fecha = DateTime.Now;
                    tipo.estado = "Activo";
                    tipoA.estado = "Inactivo";
                    db.TipoDeCambio.Add(tipo);
                    db.SaveChanges();
                    return RedirectToAction("Index", "TipoDeCambio");
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    TipoDeCambio tipo = new TipoDeCambio();
                    d = model.valor * 10000;
                    b = System.Math.Truncate(d);
                    int n = int.Parse(b + "");
                    tipo.valor = n;
                    tipo.fecha = DateTime.Now;
                    tipo.estado = "Activo";
                    db.TipoDeCambio.Add(tipo);
                    db.SaveChanges();
                    return RedirectToAction("Index", "TipoDeCambio");
                }
            }
            return View("Index");
        }



    }
}