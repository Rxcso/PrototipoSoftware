using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
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

        public ActionResult AbrirCaja(string montos, string montod)
        {
            Turno turno = (Turno)Session["TurnoHoy"];
            if (turno == null) return RedirectToAction("Apertura", "Ventas");
            if (turno.estadoCaja != "Pendiente") return RedirectToAction("Apertura", "Ventas");
            double m1;
            if (double.TryParse(montos, out m1) == false) return RedirectToAction("Apertura", "Ventas");
            double mS = double.Parse(montos);
            if (double.TryParse(montod, out m1) == false) return RedirectToAction("Apertura", "Ventas");
            double mD = double.Parse(montod);
            if (mS < 0 || mD < 0) return RedirectToAction("Apertura", "Ventas");
            db.Entry(turno).State = EntityState.Modified;
            turno.MontoInicioDolares = mD;
            turno.MontoInicioSoles = mS;
            turno.estadoCaja = "Abierto";
            db.SaveChanges();
            db.Entry(turno).State = EntityState.Detached;
            Session["AperturaCompleta"] = 1;
            return View();
        }

        public ActionResult CerrarCaja(string montos, string montod)
        {
            Turno turno = (Turno)Session["TurnoHoy"];
            if (turno == null) return RedirectToAction("Cierre", "Ventas");
            if (turno.estadoCaja == "Pendiente") return RedirectToAction("Cierre", "Ventas");
            double m1;
            if (double.TryParse(montos, out m1) == false) return RedirectToAction("Cierre", "Ventas");
            double mS = double.Parse(montos);
            if (double.TryParse(montod, out m1) == false) return RedirectToAction("Cierre", "Ventas");
            double mD = double.Parse(montod);
            if (mS < 0 || mD < 0) return RedirectToAction("Cierre", "Ventas");
            db.Entry(turno).State = EntityState.Modified;
            turno.MontoFinDolares = mD;
            turno.MontoFinSoles = mS;
            turno.estadoCaja = "Cerrado";
            db.SaveChanges();
            db.Entry(turno).State = EntityState.Detached;
            Session["CierreCompleta"] = 1;
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

        public ActionResult Detalles(int id)
        {
            List<VentasXFuncion> lv = db.VentasXFuncion.Where(c => c.codVen == id).ToList();
            List<VentasXFuncion> listvxf = new List<VentasXFuncion>();
            for (int j = 0; j < lv.Count; j++)
            {
                listvxf.Add(lv[j]);
            }
            Session["ListaVentaFuncionCliente"] = listvxf;
            return View("Detalles");
        }

        public ActionResult ReporteDia()
        {
            return View();
        }

        public ActionResult LlenaOrg(string id)
        {
            int idO = int.Parse(id);
            Organizador org = db.Organizador.Find(idO);
            Session["orgPago"] = org;
            //double subtotal;
            //double total=0;
            //List<Eventos> listEv = db.Eventos.AsNoTracking().Where(c => c.idOrganizador == idO).ToList();
            //List<Eventos> listEp=new List<Eventos>();
            //for (int i = 0; i < listEv.Count; i++)
            //{
            //    subtotal = (double)listEv[i].monto_adeudado - (double)listEv[i].monto_transferir;
            //    if (subtotal > 0)
            //    {
            //        listEp.Add(listEv[i]);
            //    }
            //    total += subtotal;
            //}
            List<Pago> listP = db.Pago.Where(c => c.codOrg == idO).ToList();
            Session["Pagos"] = listP;
            //Session["Pendiente"] = total;
            //Session["EventosP"] = listEp;
            return RedirectToAction("Pago", "Ventas");
        }

        public ActionResult LlenaVend(string id)
        {
            if (id == "" || id == null) return RedirectToAction("Asignacion", "Ventas");
            string usuario = id.Replace("°", "@");
            CuentaUsuario vend = db.CuentaUsuario.Find(usuario);
            DateTime hoy = DateTime.Now;
            List<Turno> listatuvend = db.Turno.AsNoTracking().Where(c => c.usuario == usuario && c.fecha > hoy).ToList();
            Session["ListaTurnoVendedor"] = listatuvend;
            Session["vendAsig"] = vend;

            return RedirectToAction("Asignacion", "Ventas");
        }

        public ActionResult RegistrarPagos(string monto, string pend)
        {
            double m1;
            if (double.TryParse(monto, out m1) == false) return RedirectToAction("Pago", "Ventas");
            double m = double.Parse(monto);
            double pend1 = double.Parse(pend);
            if (m > pend1) return RedirectToAction("Pago", "Ventas");
            if (m <= 0) return RedirectToAction("Pago", "Ventas");
            //if (monto == "" || monto == null) return RedirectToAction("Pago", "Ventas");         
            //List<Eventos> listEp = (List<Eventos>)Session["EventosP"];
            Organizador org = (Organizador)Session["orgPago"];
            m1 = m;
            int codE = 1;
            if (Session["EventoSeleccionadoPago"] != null) codE = (int)Session["EventoSeleccionadoPago"];
            Eventos ev = db.Eventos.Find(codE);
            //for (int i = 0; i < listEp.Count; i++)
            //{
            //    if (m == 0) break;
            while (m != 0)
            {
                if (pend1 < m)
                {
                    db.Entry(ev).State = EntityState.Modified;
                    ev.monto_transferir = ev.monto_adeudado;
                    Pago pg = new Pago();
                    pg.codEvento = ev.codigo;
                    pg.codOrg = org.codOrg;
                    Pago pl = db.Pago.ToList().Last();
                    pg.codPago = pl.codPago + 1;
                    pg.descripcion = "Pago hecho ha " + org.nombOrg;
                    //pg.Eventos = listEp[i];
                    pg.fecha = DateTime.Now;
                    pg.monto = pend1;
                    db.Pago.Add(pg);
                    db.SaveChanges();
                    m -= pend1;
                }
                else
                {
                    db.Entry(ev).State = EntityState.Modified;
                    ev.monto_transferir = ev.monto_transferir + m;
                    //db.SaveChanges();
                    Pago pg = new Pago();
                    pg.codEvento = ev.codigo;
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
            Session["Pendiente"] = (double)Session["Pendiente"] - m1;
            //double subtotal;
            //List<Eventos> listEv = db.Eventos.AsNoTracking().Where(c => c.idOrganizador == org.codOrg).ToList();
            //List<Eventos> listEpa = new List<Eventos>();
            //for (int i = 0; i < listEv.Count; i++)
            //{
            //    subtotal = (double)listEv[i].monto_adeudado - (double)listEv[i].monto_transferir;
            //    if (subtotal > 0)
            //    {
            //        listEpa.Add(listEv[i]);
            //    }
            //}
            //Session["EventosP"] = listEpa;
            return View();
        }
    }
}