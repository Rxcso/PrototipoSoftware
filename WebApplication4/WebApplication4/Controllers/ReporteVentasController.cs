using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class ReporteVentasController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: ReporteVentas
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ReporteV1(string fd, string fh)
        {
            double to = 0;
            DateTime dt1 = DateTime.Parse(fd);
            DateTime dt2 = DateTime.Parse(fh);
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            DateTime di = dt1;
            List<ReporteModel.ReporteVentas1Model> lr = new List<ReporteModel.ReporteVentas1Model>();
            List<CuentaUsuario> lv = db.CuentaUsuario.Where(c => c.codPerfil == 2 && c.estado == true).ToList();
            for (int j = 0; j < nd; j++)
            {
                for (int i = 0; i < lv.Count; i++)
                {
                    ReporteModel.ReporteVentas1Model r = new ReporteModel.ReporteVentas1Model();
                    r.fecha = di.Date;
                    String no = lv[i].usuario;
                    r.nombre = lv[i].nombre;
                    r.codigo = lv[i].usuario;
                    List<Turno> lt = db.Turno.Where(c => c.usuario == no && c.fecha == di).ToList();
                    if (lt.Count > 0)
                    {
                        double total = 0;
                        Turno t = lt.First();
                        r.punto = db.PuntoVenta.Find(t.codPuntoVenta).ubicacion;
                        List<Ventas> lven = db.Ventas.Where(c => c.fecha == di && c.Estado == "Pagado" && c.vendedor == no).ToList();
                        for (int k = 0; k < lven.Count; k++)
                        {
                            total += (double)lven[i].MontoTotalSoles;
                        }
                        r.total = total;
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
            DateTime dt2 = DateTime.Parse(fh);
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            List<ReporteModel.ReporteVentas2Model> lr = new List<ReporteModel.ReporteVentas2Model>();
            List<CuentaUsuario> lv = db.CuentaUsuario.Where(c => c.codPerfil == 2 && c.estado == true).ToList();
            for (int i = 0; i < lv.Count; i++)
            {
                ReporteModel.ReporteVentas2Model r = new ReporteModel.ReporteVentas2Model();
                r.codigo = lv[i].usuario;
                r.nombre = lv[i].nombre;
                double total = 0;
                DateTime di = dt1;
                for (int j = 0; j < nd; j++)
                {
                    string us = lv[i].usuario;
                    List<Ventas> lven = db.Ventas.Where(c => c.fecha == di && c.Estado == "Pagado" && c.vendedor == us).ToList();
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
            DateTime dt2 = DateTime.Parse(fh);
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            List<ReporteModel.ReporteVentas3Model> lr = new List<ReporteModel.ReporteVentas3Model>();
            List<Eventos> lev = db.Eventos.ToList();

            for (int i = 0; i < lev.Count; i++)
            {
                ReporteModel.ReporteVentas3Model r = new ReporteModel.ReporteVentas3Model();
                r.codigo = lev[i].codigo;
                r.nombre = lev[i].nombre;
                r.organizador = db.Organizador.Find(lev[i].idOrganizador).nombOrg;
                int ce = lev[i].codigo;
                List<Funcion> lf = db.Funcion.Where(c => c.codEvento == ce).ToList();
                for (int k = 0; k < lf.Count; k++)
                {
                    double total = 0;
                    int totale = 0;
                    //        DateTime di = dt1;

                    //        for (int j = 0; j < nd; j++)
                    //        {
                    //            List<Ventas> lven = db.Ventas.Where(c => c.fecha == di && c.Estado == "Pagado").ToList();

                    //            for (int t = 0; t < lven.Count; t++)
                    //            {
                    //                total += (double)lven[t].MontoTotalSoles;
                    //            }
                    //            di = di.AddDays(1);
                    //        }
                    //        r.total = total;
                    r.funcion = db.Funcion.Find(lf[k].codFuncion).horaIni.Value.ToString(@"hh\:mm\:ss"); ;
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
            DateTime dt2 = DateTime.Parse(fh);
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
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
                        List<Ventas> lven = db.Ventas.Where(c => c.fecha == di && c.Estado == "Pagado" && c.vendedor == t.usuario).ToList();
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