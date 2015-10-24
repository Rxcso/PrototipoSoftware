using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            double subtotal;
            double total=0;
            List<Eventos> listEv = db.Eventos.AsNoTracking().Where(c => c.idOrganizador == idO).ToList();
            List<Eventos> listEp=new List<Eventos>();
            for (int i = 0; i < listEv.Count; i++)
            {
                subtotal = (double)listEv[i].monto_adeudado - (double)listEv[i].monto_transferir;
                if (subtotal > 0)
                {
                    listEp.Add(listEv[i]);
                }
                total += subtotal;
            }
            List<Pago> listP = db.Pago.Where(c => c.codOrg == idO).ToList();
            Session["Pagos"] = listP;
            Session["Pendiente"] = total;
            Session["EventosP"] = listEp;
            return RedirectToAction("Pago", "Ventas");
        }

        public ActionResult RegistrarPagos(string monto)
        {
            double m1;
            if (double.TryParse(monto,out m1) == false) return RedirectToAction("Pago", "Ventas");
            double m = double.Parse(monto);
            double pend = (double)Session["Pendiente"];
            if (m > pend) return RedirectToAction("Pago", "Ventas");
            if (monto == "" || monto == null) return RedirectToAction("Pago", "Ventas");         
            List<Eventos> listEp = (List<Eventos>)Session["EventosP"];
            Organizador org = (Organizador)Session["orgPago"];
            for (int i = 0; i < listEp.Count; i++)
            {
                if (m == 0) break;
                double pendE=(double)listEp[i].monto_adeudado - (double)listEp[i].monto_transferir;
                if (pendE < m)
                {
                    db.Entry(listEp[i]).State = EntityState.Modified;
                    listEp[i].monto_transferir = listEp[i].monto_adeudado;
                    Pago pg = new Pago();
                    pg.codEvento = listEp[i].codigo;
                    pg.codOrg = org.codOrg;
                    Pago pl = db.Pago.ToList().Last();
                    pg.codPago = pl.codPago + 1;
                    pg.descripcion = "Pago hecho ha " + org.nombOrg;
                    //pg.Eventos = listEp[i];
                    pg.fecha = DateTime.Now;
                    pg.monto = pendE;
                    db.Pago.Add(pg);
                    db.SaveChanges();
                    m -= pendE;
                }
                else
                {
                    db.Entry(listEp[i]).State = EntityState.Modified;
                    listEp[i].monto_transferir = listEp[i].monto_transferir + m;
                    //db.SaveChanges();
                    Pago pg = new Pago();
                    pg.codEvento = listEp[i].codigo;
                    pg.codOrg = org.codOrg;
                    Pago pl = db.Pago.ToList().Last();
                    pg.codPago = pl.codPago + 1;
                    pg.descripcion = "Pago hecho ha " + org.nombOrg;
                    //pg.Eventos = listEp[i];
                    pg.fecha = DateTime.Now;
                    pg.monto = m;
                    //pg.Organizador = org;
                    db.Pago.Add(pg);
                    db.SaveChanges();
                    m = 0;
                }
            }
            List<Pago> listP = db.Pago.Where(c => c.codOrg == org.codOrg).ToList();
            Session["Pagos"] = listP;
            Session["Pendiente"] = (double)Session["Pendiente"] - m;
            double subtotal;
            List<Eventos> listEv = db.Eventos.AsNoTracking().Where(c => c.idOrganizador == org.codOrg).ToList();
            List<Eventos> listEpa = new List<Eventos>();
            for (int i = 0; i < listEv.Count; i++)
            {
                subtotal = (double)listEv[i].monto_adeudado - (double)listEv[i].monto_transferir;
                if (subtotal > 0)
                {
                    listEpa.Add(listEv[i]);
                }
            }
            Session["EventosP"] = listEpa;
             return View();
        }
    }
}