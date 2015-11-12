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

        public JsonResult AbrirCaja(string montos, string montod)
        {
            Turno turno = (Turno)Session["TurnoHoy"];
            if (turno == null) return Json("No tienes un turno asignado", JsonRequestBehavior.AllowGet);
            if (turno.estadoCaja != "Pendiente") return Json("Caja ya ha sido abierta", JsonRequestBehavior.AllowGet);
            double m1;
            if (double.TryParse(montos, out m1) == false) return Json("Monto ingresado invalido", JsonRequestBehavior.AllowGet);
            double mS = double.Parse(montos);
            if (double.TryParse(montod, out m1) == false) return Json("Monto ingresado invalido", JsonRequestBehavior.AllowGet);
            double mD = double.Parse(montod);
            if (mS < 0 || mD < 0) return Json("Los montos ingresados deben ser mayor o iguales a cero", JsonRequestBehavior.AllowGet);
            db.Entry(turno).State = EntityState.Modified;
            turno.MontoInicioDolares = mD;
            turno.MontoInicioSoles = mS;
            turno.estadoCaja = "Abierto";
            db.SaveChanges();
            db.Entry(turno).State = EntityState.Detached;
            Session["AperturaCompleta"] = 1;
            return Json("Caja Abierta", JsonRequestBehavior.AllowGet);
        }

        public JsonResult CerrarCaja(string montos, string montod)
        {
            Turno turno = (Turno)Session["TurnoHoy"];
            if (turno == null) return Json("No tienes un turno asignado", JsonRequestBehavior.AllowGet);
            if (turno.estadoCaja == "Pendiente" || turno.estadoCaja == "Cerrado") return Json("Caja no ha sido abierta o caja ya esta cerrada", JsonRequestBehavior.AllowGet);
            double m1;
            if (double.TryParse(montos, out m1) == false) return Json("Monto ingresado invalido", JsonRequestBehavior.AllowGet);
            double mS = double.Parse(montos);
            if (double.TryParse(montod, out m1) == false) return Json("Monto ingresado invalido", JsonRequestBehavior.AllowGet);
            double mD = double.Parse(montod);
            if (mS < 0 || mD < 0) return Json("Los montos ingresados deben ser mayor o iguales a cero", JsonRequestBehavior.AllowGet);
            db.Entry(turno).State = EntityState.Modified;
            turno.MontoFinDolares = mD;
            turno.MontoFinSoles = mS;
            turno.estadoCaja = "Cerrado";
            db.SaveChanges();
            db.Entry(turno).State = EntityState.Detached;
            Session["CierreCompleta"] = 1;
            return Json("Caja Cerrada", JsonRequestBehavior.AllowGet);
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

        public ActionResult PagoOrganizador()
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
            List<Pago> listP = db.Pago.Where(c => c.codOrg == idO && c.monto>0).ToList();
            Session["Pagos"] = listP;
            //Session["Pendiente"] = total;
            //Session["EventosP"] = listEp;
            return RedirectToAction("Pago", "Ventas");
        }

        public ActionResult LlenaOrg2(string id)
        {
            int idO = int.Parse(id);
            Organizador org = db.Organizador.Find(idO);
            Session["orgPago2"] = org;
            List<Pago> listP = db.Pago.Where(c => c.codOrg == idO && c.monto<0).ToList();
            Session["Pagos2"] = listP;
            Session["Pendiente2"] = null;
            return RedirectToAction("PagoOrganizador", "Ventas");
        }

        public ActionResult LlenaVend(string id)
        {
            if (id == "" || id == null) return RedirectToAction("Asignacion", "Ventas");
            string usuario = id.Replace("°", "@");
            CuentaUsuario vend = db.CuentaUsuario.Find(usuario);
            DateTime hoy = DateTime.Now.Date;
            List<Turno> listatuvend = db.Turno.AsNoTracking().Where(c => c.usuario == usuario && c.fecha >= hoy).ToList();
            Session["ListaTurnoVendedor"] = listatuvend;
            Session["vendAsig"] = vend;

            return RedirectToAction("Asignacion", "Ventas");
        }

        public JsonResult RegistrarPagos(string monto, string pend)
        {
            double m1;
            if (double.TryParse(monto, out m1) == false) return Json("monto invalido", JsonRequestBehavior.AllowGet);
            double m = double.Parse(monto);
            double pend1 = double.Parse(pend);
            if (m <= 0)
            {
                return Json("monto ingresado debe ser mayor que 0", JsonRequestBehavior.AllowGet);
            }
            if (m > pend1) return Json("monto ingresado debe ser menor o igual al monto pendiente", JsonRequestBehavior.AllowGet);
            //if (m <= 0) return RedirectToAction("Pago", "Ventas");
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
                    //Pago pl = db.Pago.ToList().Last();
                    //pg.codPago = pl.codPago + 1;
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
                    //Pago pl = db.Pago.ToList().Last();
                    //pg.codPago = pl.codPago + 1;
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
            List<Pago> listP = db.Pago.Where(c => c.codOrg == org.codOrg && c.monto>0).ToList();
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
            return Json("Pago Registrado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegistrarPagos2(string monto, string pend)
        {
            double m1;
            if (double.TryParse(monto, out m1) == false) return Json("monto invalido", JsonRequestBehavior.AllowGet);
            double m = double.Parse(monto);
            double pend1 = double.Parse(pend);
            if (m <= 0)
            {
                return Json("monto ingresado debe ser mayor que 0", JsonRequestBehavior.AllowGet);
            }
            if (m > pend1) return Json("monto ingresado debe ser menor o igual al monto pendiente", JsonRequestBehavior.AllowGet);
            Organizador org = (Organizador)Session["orgPago2"];
            m1 = m;
            int codE = 1;
            if (Session["EventoSeleccionadoPago2"] != null) codE = (int)Session["EventoSeleccionadoPago2"];
            Eventos ev = db.Eventos.Find(codE);

            Pago pg = new Pago();
            pg.codEvento = ev.codigo;
            pg.codOrg = org.codOrg;
            //Pago pl = db.Pago.ToList().Last();
            //pg.codPago = pl.codPago + 1;
            pg.descripcion = "Pago hecho por el " + org.nombOrg;
            pg.fecha = DateTime.Now;
            pg.monto = -m; 
            db.Pago.Add(pg);
            db.SaveChanges();
            List<Pago> listP = db.Pago.Where(c => c.codOrg == org.codOrg && c.monto<0).ToList();
            Session["Pagos2"] = listP;
            Session["Pendiente2"] = (double)Session["Pendiente2"] - m1;
            return Json("Pago Registrado", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DevolucionIndex()
        {
            return RedirectToAction("Devolucion", "Ventas");
        }

        public ActionResult SearchDoc(string doc) 
        {
            if (doc == "")
            {                
                Session["Bus"] = null;
                return RedirectToAction("Devolucion", "Ventas");
            }     

            List<DevolucionModel> devolucion = new List<DevolucionModel>();
            List<VentasXFuncion> listaRealVxf = new List<VentasXFuncion>();
            List<VentasXFuncion> vxf = null;
            List<CuentaUsuario> usuario = db.CuentaUsuario.Where(u => u.codDoc == doc).ToList();//saco el usuario
            List<Ventas> v = db.Ventas.Where(ven => ven.codDoc == doc && ven.Estado == "Pagado").ToList();
            //saco todas las compras realizadas por dicho usuario que tengan estado Pagado
            if (v != null) 
            {
                for (int i = 0; i < v.Count; i++)
                {
                    //hallo la lista de VentasXFuncion de cada venta    
                    int codiguito = v[i].codVen;
                    vxf = db.VentasXFuncion.Where(venxf => venxf.codVen == codiguito).ToList();
                    if (vxf != null) 
                    {                        
                        for (int j = 0; j < vxf.Count; j++)
                        {
                            int codiguitoF = vxf[j].codFuncion;
                            //filtro
                            Funcion f = db.Funcion.Find(codiguitoF);
                            if (f.estado != "CANCELADO" && f.estado != "POSTERGADO") {
                                vxf.RemoveAt(j);
                                j--;
                            }
                            //logica de que en caso de postergacion no se permita devolver el dinero                                
                            if (f.estado == "POSTERGADO")
                            {
                                Eventos ev = db.Eventos.Find(f.codEvento);
                                if(!ev.devolverPostergacion){
                                    vxf.RemoveAt(j);
                                    j--;
                                }
                            }
                            
                            /*List<Funcion> f = db.Funcion.Where(fun => fun.codFuncion == codiguitoF && 
                                (fun.estado == "Cancelado" || fun.estado == "Postergado")).ToList();
                            if (f == null) { 
                                vxf.RemoveAt(j);
                                j--;
                            }else
                                if (f[0].estado == "POSTERGADO")
                                {
                                    Eventos ev = db.Eventos.Find(f[0].codEvento);
                                    if (!ev.devolverPostergacion) { 
                                        vxf.RemoveAt(j);
                                        j--;
                                    }
                                }*/

                            //si el evento asociado a ese VXF no es postergado ni cancelado, lo borro
                        }
                            //la lista que mantendrá absolutamente todos los VXF de todas las ventas
                            listaRealVxf.AddRange(vxf);
                    }                    
                }
                if (listaRealVxf != null)
                {
                    for (int i = 0; i < listaRealVxf.Count; i++)
                    {
                        int codiguitoV = listaRealVxf[i].codVen;
                        int codiguitoF = listaRealVxf[i].codFuncion;
                        //saco todos los DetallesVentas de todos los VXF
                        List<DetalleVenta> detVen = db.DetalleVenta.Where(dv => dv.codVen == codiguitoV &&
                        dv.codFuncion == codiguitoF).ToList();

                        for (int j = 0; j < detVen.Count; j++)
                        {
                            //se llena la lista de devoluciones por cada detalle de venta
                            DevolucionModel d = new DevolucionModel();
                            d.codDev = detVen[j].codDetalleVenta;
                            d.numDoc = int.Parse(doc);
                            if(usuario.Count==0)
                                d.nombre = "--";
                            else
                                d.nombre = usuario[0].apellido + ", " + usuario[0].nombre;

                            int codigoFuncion = detVen[j].codFuncion;
                            List<Funcion> funAux = db.Funcion.Where(fu => fu.codFuncion == codigoFuncion).ToList();

                            int codigoEvento = funAux[0].codEvento;
                            List<Eventos> eventosAux = db.Eventos.Where(e => e.codigo == codigoEvento).ToList();

                            d.fecha = (DateTime)funAux[0].fecha;
                            d.hora = (DateTime)funAux[0].horaIni;
                            d.evento = eventosAux[0].nombre;
                            d.cantAsientos = (int)detVen[j].cantEntradas;
                            d.monto = (double)detVen[j].total;
                            d.estado = funAux[0].estado;

                            codigoFuncion = detVen[0].codFuncion;
                            int codigoDetVen = detVen[0].codDetalleVenta;

                            PrecioEvento pe = db.PrecioEvento.Find(detVen[0].codPrecE);
                            ZonaEvento ze = db.ZonaEvento.Find(pe.codZonaEvento);
                            /*
                            List<AsientosXFuncion> axf = db.AsientosXFuncion.Where(a => a.codFuncion == codigoFuncion && a.codDetalleVenta == codigoDetVen).ToList();
                            
                            int codigoAsiento = axf[0].codAsiento;
                            List<Asientos> asientos = db.Asientos.Where(a => a.codAsiento == codigoAsiento).ToList();

                            int codigoZona = asientos[0].codZona;
                            List<ZonaEvento> ze = db.ZonaEvento.Where(z => z.codZona == codigoZona).ToList();

                            d.zona = ze[0].nombre;*/
                            d.zona = ze.nombre;
                            devolucion.Add(d);
                        }
                    }
                }                
            }                       
                   
            if (devolucion != null) Session["BusquedaDev"] = devolucion;
            else Session["BusquedaDev"] = null;
            return RedirectToAction("Devolucion", "Ventas");
            //return View("Devolucion");
        }

        public ActionResult Devolver(string fila)
        {
            int id = int.Parse(fila);
            List<DetalleVenta> dv = db.DetalleVenta.Where(det => det.codDetalleVenta == id).ToList();
            List<AsientosXFuncion> af = db.AsientosXFuncion.Where(axf => axf.codDetalleVenta == id && axf.codFuncion == dv[0].codFuncion).ToList();
            List<Funcion> f = db.Funcion.Where(fun => fun.codFuncion == dv[0].codFuncion).ToList();
            List<Eventos> ev = db.Eventos.Where(e => e.codigo == f[0].codEvento).ToList();
            for (int i = 0; i < af.Count; i++)
            {
                af[i].estado = "DEVUELTO";
            }
            dv[0].entradasDev = dv[0].cantEntradas;
            ev[0].monto_transferir -= (double)dv[0].total;
            db.SaveChanges();

            /*d.codDev = detVen[j].codDetalleVenta;
            d.numDoc = int.Parse(doc);
            d.nombre = usuario[0].apellido + ", " + usuario[0].nombre;
            List<Funcion> funAux = db.Funcion.Where(fu => fu.codFuncion == detVen[j].codFuncion).ToList();
            List<Eventos> eventosAux = db.Eventos.Where(e => e.codigo == funAux[0].codEvento).ToList();
            d.fecha = (DateTime)funAux[0].fecha;
            d.hora = (DateTime)funAux[0].horaIni;
            d.evento = eventosAux[0].nombre;
            d.cantAsientos = (int)detVen[j].cantEntradas;
            d.monto = (double)detVen[j].total;
            d.estado = funAux[0].estado;*/

            //logica de devolucion!
            return View("Devolucion");
        }
        
        public ActionResult DevolverTodo()
        {
            DetalleVenta dv=(DetalleVenta)Session["DetalleVenta"];
            Eventos ev = (Eventos)Session["EventoDev"];
            //ev.monto_transferir-=
            /*Session["DetalleVenta"]
            Session["VentaXFunDev"]
            Session["VentasDev"]
            Session["ListaAsientos"] = axf;
            Session["BusquedaDev"] = devolucionModel
            Session["FuncionDev"]
            Session["EventoDev"]
            */



            return View("Devolucion");
        }

        public ActionResult VerDetalle(string detVen)
        {
            //Session["Bus"] = null;
            int id = int.Parse(detVen);
            ViewBag.id = id;
            TempData["codigoDet"] = id;
            
            DetalleVenta detalleVen = db.DetalleVenta.Find(id);
            Session["DetalleVenta"] = detalleVen;
            VentasXFuncion venxf = db.VentasXFuncion.Where(v=>v.codFuncion==detalleVen.codFuncion && v.codVen==detalleVen.codVen).ToList()[0];
            Session["VentaXFunDev"] = venxf;
            Ventas ven = db.Ventas.Find(venxf.codVen);
            Session["VentasDev"] = ven;
            List<AsientosXFuncion> axf = db.AsientosXFuncion.Where(a=>a.codFuncion==detalleVen.codFuncion && a.codDetalleVenta==detalleVen.codDetalleVenta).ToList();
            Session["ListaAsientos"] = axf;
            Funcion funDev = db.Funcion.Find(detalleVen.codFuncion);
            Session["FuncionDev"] = funDev;
            Eventos eventoDev = db.Eventos.Find(funDev.codEvento);
            Session["EventoDev"] = eventoDev;

            return View("VerDetalle");
        }

    }
}