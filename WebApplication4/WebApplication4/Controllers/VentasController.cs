using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class VentasController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: Ventas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Apertura()
        {
            return View();
        }

        public ActionResult Cierre()
        {
            return View();
        }

        public ActionResult Entrega()
        {
            return View();
        }

        public ActionResult Devolucion()
        {
            return View();
        }

        public ActionResult Pago()
        {
            return View();
        }

        public ActionResult ReporteDia()
        {
            return View();
        }

        public ActionResult LlenaOrg(string id)
        {
            int idO = int.Parse(id);
            Organizador org=db.Organizador.Find(idO);
            Session["orgPago"] = org;
            return RedirectToAction("Pado", "Ventas");
        }
    }
}