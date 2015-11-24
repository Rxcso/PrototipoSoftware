using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class ReporteVentasController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: ReporteVentas
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LimpiaR1()
        {
            Session["ReporteVentasTotal"] = null;
            Session["ReporteTotal"] = null;
            return Json("Reporte Limpio", JsonRequestBehavior.AllowGet);
        }

        public JsonResult LimpiaR2()
        {
            Session["ReporteVentasTotal2"] = null;
            Session["ReporteTotal2"] = null;
            return Json("Reporte Limpio", JsonRequestBehavior.AllowGet);
        }

        public JsonResult LimpiaR3()
        {
            Session["ReporteVentasTotal3"] = null;
            Session["ReporteTotal3"] = null;
            return Json("Reporte Limpio", JsonRequestBehavior.AllowGet);
        }

        public JsonResult LimpiaR4()
        {
            Session["ReporteVentasTotal4"] = null;
            Session["ReporteTotal4"] = null;
            return Json("Reporte Limpio", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReporteV1(string fd, string fh)
        {
            double to = 0;
            DateTime dt1 = DateTime.Parse(fd);
            DateTime dt2 = DateTime.Now;
            if (fh != null && fh != "")
            {
                dt2 = DateTime.Parse(fh);
            }
            Session["FechaRI"] = dt1;
            Session["FechaRF"] = dt2;
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            DateTime di = dt1;
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            List<ReporteModel.ReporteVentas1Model> lr = new List<ReporteModel.ReporteVentas1Model>();
            List<CuentaUsuario> lv = db.CuentaUsuario.Where(c => c.codPerfil == 2 && c.estado == true).ToList();
            for (int j = 0; j < nd; j++)
            {
                for (int i = 0; i < lv.Count; i++)
                {
                    ReporteModel.ReporteVentas1Model r = new ReporteModel.ReporteVentas1Model();
                    r.fecha = di.Date;
                    DateTime dat = di.Date;
                    String no = lv[i].usuario;
                    r.nombre = lv[i].nombre;
                    r.codigo = lv[i].codDoc;
                    List<Turno> lt = db.Turno.Where(c => c.usuario == no && c.fecha == dat).ToList();
                    if (lt.Count > 0)
                    {
                        double total = 0;
                        double dev = 0;
                        Turno t = lt.First();
                        r.punto = db.PuntoVenta.Find(t.codPuntoVenta).ubicacion;
                        List<Ventas> lven2 = db.Ventas.Where(c => c.Estado == "Pagado" && c.vendedor == no).ToList();
                        List<Ventas> lven = lven2.Where(c => c.fecha.Value.Date == dat).ToList();
                        List<LogDevoluciones> llog1 = db.LogDevoluciones.Where(c => c.codVendedor == no).ToList();
                        List<LogDevoluciones> llog2 = llog1.Where(c => c.fechaDev.Value.Date == dat).ToList();
                        for (int k = 0; k < lven.Count; k++)
                        {
                            total += (double)lven[k].MontoTotalSoles;
                        }
                        for (int w = 0; w < llog2.Count; w++)
                        {
                            dev += (double)llog2[w].montoDev;
                        }
                        r.total = total;
                        r.devtotal = dev;
                        to += total;
                        lr.Add(r);
                    }
                }
                di = di.AddDays(1);
            }
            Session["ReporteVentasTotal"] = lr;
            Session["ReporteTotal"] = to;
            return Json("Reporte Generado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReporteV2(string fd, string fh)
        {
            double to = 0;
            DateTime dt1 = DateTime.Parse(fd);
            DateTime dt2 = DateTime.Now;
            if (fh != null && fh != "")
            {
                dt2 = DateTime.Parse(fh);
            }
            Session["FechaRI2"] = dt1;
            Session["FechaRF2"] = dt2;
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            List<ReporteModel.ReporteVentas2Model> lr = new List<ReporteModel.ReporteVentas2Model>();
            List<CuentaUsuario> lv = db.CuentaUsuario.Where(c => c.codPerfil == 2 && c.estado == true).ToList();
            for (int i = 0; i < lv.Count; i++)
            {
                ReporteModel.ReporteVentas2Model r = new ReporteModel.ReporteVentas2Model();
                r.codigo = lv[i].usuario;
                string us = lv[i].usuario;
                r.nombre = lv[i].nombre;
                List<Ventas> lven2 = db.Ventas.Where(c => c.Estado == "Pagado" && c.vendedor == us).ToList();
                double total = 0;
                DateTime di = dt1;
                for (int j = 0; j < nd; j++)
                {
                    DateTime dat = di.Date;
                    List<Ventas> lven = lven2.Where(c => c.fecha.Value.Date == dat).ToList();
                    for (int k = 0; k < lven.Count; k++)
                    {
                        total += (double)lven[k].MontoTotalSoles;
                    }
                    di = di.AddDays(1);
                }
                r.total = total;
                to += total;
                lr.Add(r);
            }
            Session["ReporteVentasTotal2"] = lr;
            Session["ReporteTotal2"] = to;
            return Json("Reporte Generado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReporteV3(string fd, string fh)
        {
            double to = 0;
            DateTime dt1 = DateTime.Parse(fd);
            DateTime dt2 = DateTime.Now;
            if (fh != null && fh != "")
            {
                dt2 = DateTime.Parse(fh);
            }
            Session["FechaRI3"] = dt1;
            Session["FechaRF3"] = dt2;
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            List<ReporteModel.ReporteVentas3Model> lr = new List<ReporteModel.ReporteVentas3Model>();
            List<Eventos> lv2 = db.Eventos.ToList();
            List<Eventos> lev = lv2.Where(r => r.estado == "Activo"  && r.fecha_inicio>dt1.Date && r.fecha_inicio<dt2.Date).ToList(); 
            for (int i = 0; i < lev.Count; i++)
            {
                int ce = lev[i].codigo;
                List<Funcion> lf = db.Funcion.Where(c => c.codEvento == ce && c.estado == "ACTIVO").ToList();
                for (int k = 0; k < lf.Count; k++)
                {
                    ReporteModel.ReporteVentas3Model r = new ReporteModel.ReporteVentas3Model();
                    r.codigo = lev[i].codigo;
                    r.nombre = lev[i].nombre;
                    r.organizador = db.Organizador.Find(lev[i].idOrganizador).nombOrg;
                    int cf=lf[k].codFuncion;
                    double total = 0;
                    int totale = 0;
                    List<VentasXFuncion> lvxf = db.VentasXFuncion.Where(c => c.codFuncion == cf).ToList();
                    DateTime di = dt1;
                    DateTime dat = di.Date;

                    for (int j = 0; j < lvxf.Count; j++)
                    {
                        if (lvxf[j].cantEntradas > 0)
                        {
                            totale += (int)lvxf[j].cantEntradas;
                            total += (double)lvxf[j].total;
                        }
                    }
                        r.total = total;
                        r.funcion = db.Funcion.Find(lf[k].codFuncion).fecha.Value.ToString("dd/MM/yyyy") + " - " + db.Funcion.Find(lf[k].codFuncion).horaIni.Value.ToString(@"hh\:mm\:ss"); 
                    r.total = total;
                    r.cant = totale;
                    to += total;
                    lr.Add(r);
                }
            }
            Session["ReporteVentasTotal3"] = lr;
            Session["ReporteTotal3"] = to;
            return Json("Reporte Generado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReporteV4(string fd, string fh)
        {
            double to = 0;
            DateTime dt1 = DateTime.Parse(fd);
            DateTime dt2 = DateTime.Now;
            if (fh != null && fh != "")
            {
                dt2 = DateTime.Parse(fh);
            }
            TimeSpan ts = dt2.Subtract(dt1);
            Session["FechaRI4"] = dt1;
            Session["FechaRF4"] = dt2;
            int nd = (int)ts.Days;
            nd = nd + 1;
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            List<ReporteModel.ReporteVentas4Model> lr = new List<ReporteModel.ReporteVentas4Model>();
            List<PuntoVenta> lv = db.PuntoVenta.Where(c => c.estaActivo == true).ToList();
            for (int i = 0; i < lv.Count; i++)
            {
                ReporteModel.ReporteVentas4Model r = new ReporteModel.ReporteVentas4Model();
                r.codigo = lv[i].codPuntoVenta;
                r.nombre = lv[i].ubicacion;
                r.distrito = db.Region.Find(lv[i].idRegion).nombre;
                r.provincia = db.Region.Find(lv[i].idProvincia).nombre;
                double total = 0;
                DateTime di = dt1;
                for (int j = 0; j < nd; j++)
                {
                    int cp = lv[i].codPuntoVenta;
                    List<Turno> lt = db.Turno.Where(c => c.codPuntoVenta == cp && c.fecha == di).ToList();
                    if (lt.Count > 0)
                    {
                        Turno t = lt.First();
                        List<Ventas> lven2 = db.Ventas.Where(c => c.Estado == "Pagado" && c.vendedor == t.usuario).ToList();
                        DateTime dat = di.Date;
                        List<Ventas> lven = lven2.Where(c => c.fecha.Value.Date == dat).ToList();
                        for (int k = 0; k < lven.Count; k++)
                        {
                            total += (double)lven[k].MontoTotalSoles;
                        }
                    }
                    di = di.AddDays(1);
                }
                r.total = total;
                to += total;
                lr.Add(r);
            }
            Session["ReporteVentasTotal4"] = lr;
            Session["ReporteTotal4"] = to;
            return Json("Reporte Generado", JsonRequestBehavior.AllowGet);
        }
    }
}