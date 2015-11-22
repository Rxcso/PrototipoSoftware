using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Diagnostics;
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

        [HttpGet]
        [Authorize(Roles = "Vendedor")]
        public ActionResult CarritoVentas()
        {
            if (Session["CarritoVendedor"] != null)
            {
                List<PaqueteEntradas> carrito = (List<PaqueteEntradas>)Session["CarritoVendedor"];
                List<CarritoItem> item = new List<CarritoItem>();
                foreach (PaqueteEntradas paquete in carrito)
                {
                    Eventos evento = db.Eventos.Find(paquete.idEvento);
                    PeriodoVenta periodo = db.PeriodoVenta.Where(c => c.codEvento == paquete.idEvento && c.fechaInicio <= DateTime.Today && DateTime.Today <= c.fechaFin).First();
                    CarritoItem cItem = new CarritoItem();
                    cItem.idEvento = paquete.idEvento;
                    cItem.idFuncion = paquete.idFuncion;
                    cItem.idZona = paquete.idZona;
                    cItem.nombreEvento = db.Eventos.Find(paquete.idEvento).nombre;
                    Funcion funcion = db.Funcion.Find(paquete.idFuncion);
                    cItem.fecha = (DateTime)funcion.fecha;
                    cItem.hora = (DateTime)funcion.horaIni;
                    cItem.zona = db.ZonaEvento.Find(paquete.idZona).nombre;
                    cItem.precio = (double)db.PrecioEvento.Where(c => c.codZonaEvento == paquete.idZona && c.codPeriodoVenta == periodo.idPerVent).First().precio * paquete.cantEntradas;
                    cItem.filas = paquete.filas;
                    cItem.columnas = paquete.columnas;
                    cItem.tieneAsientos = paquete.tieneAsientos;
                    cItem.cantidad = paquete.cantEntradas;
                    item.Add(cItem);
                }
                Session["CarritoItemVentas"] = item;
                ViewBag.Carrito = item;
            }
            return View();
        }
        private Promociones CalculaMejorPromocionTarjeta(int codEvento, int idBanco, int tipoTarjeta)
        {
            try
            {
                //busco las promociones que se encuentren activas
                List<Promociones> promociones = db.Promociones.Where(c => c.codEvento == codEvento && c.codBanco == idBanco && c.codTipoTarjeta == tipoTarjeta && c.estado == true && c.fechaIni <= DateTime.Today && DateTime.Today <= DateTime.Today).ToList();
                promociones.Sort((a, b) => ((double)a.descuento).CompareTo((double)b.descuento));
                return promociones.Last();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Vendedor")]
        public ActionResult VenderEntrada()
        {
            if (Session["CarritoItemVentas"] != null)
            {
                //saco el carrito del session
                List<CarritoItem> carrito = (List<CarritoItem>)Session["CarritoItemVentas"];
                //lista de bancos
                List<Banco> bancos = db.Banco.ToList();
                ViewBag.Bancos = new SelectList(bancos, "codigo", "nombre");
                //lista de tarjetas
                List<TipoTarjeta> tipoTarjeta = db.TipoTarjeta.ToList();
                ViewBag.TipoTarjeta = new SelectList(tipoTarjeta, "idTipoTar", "nombre");
                List<Promociones> listaPromociones = new List<Promociones>();
                double total = 0;
                double descuento = 0;
                foreach (CarritoItem item in carrito)
                {
                    total += item.precio;
                    Promociones promocion = CalculaMejorPromocionTarjeta(item.idEvento, bancos.First().codigo, tipoTarjeta.First().idTipoTar);
                    if (promocion == null)
                    {
                        Promociones dummy = new Promociones();
                        dummy.codPromo = -1;
                        listaPromociones.Add(dummy);
                    }
                    else
                    {
                        descuento += item.precio * promocion.descuento.Value / 100;
                        listaPromociones.Add(promocion);
                    }
                }
                ViewBag.Descuento = 0;
                ViewBag.Promociones = listaPromociones;
                ViewBag.Total = total;
                ViewBag.Pagar = total - 0;
                ViewBag.Mes = Fechas.Mes();
                ViewBag.AnVen = Fechas.Anio();
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay items en el carrito.";
            return RedirectToAction("CarritoVentas");
        }

        private bool ValidacionesCompra(ComprarEntradaModel model)
        {
            bool ind = true;
            //Verficar que la tarjeta pertenezca al banco
            Banco banco = db.Banco.Find(model.idBanco);
            string identificador = "" + banco.identificador;
            string tarjeta = model.NumeroTarjeta;
            string comparador = tarjeta.Substring(0, identificador.Length);
            int pertenece = comparador.CompareTo(identificador);
            if (pertenece != 0)
            {
                ind = false;
                ModelState.AddModelError("NumeroTarjeta", "El numero de tarjeta no pertenece al banco indicado.");
            }
            //verificar que no haya vencido la tarjeta
            int mes = model.Mes;
            int anio = model.AnioVen;
            DateTime hoy = DateTime.Today;
            if ((mes < hoy.Month && anio == hoy.Year))
            {
                ind = false;
                ModelState.AddModelError("AnioVen", "La tarjeta ya venció.");
            }
            return ind;
        }

        private bool validacionVenta(VenderEntradaModel model)
        {
            bool indicador = true;
            //si es una compra mixta
            if (model.MontoEfe <= model.MontoPagar)
            {
                //usa tarjeta, verificar que hayan datos de la tarjeta
                if (String.IsNullOrEmpty(model.NumeroTarjeta))
                {//reviso si no hay una tarjeta seleccionadad
                    ModelState.AddModelError("NumeroTarjeta", "El campo Nro. de Tarjeta: es obligatorio.");
                    indicador = false;
                }
                else
                {//si hay una tarjeta tengo que ver si pertenece al banco
                    ComprarEntradaModel compra = new ComprarEntradaModel();
                    compra.idBanco = (int)model.idBanco;
                    compra.NumeroTarjeta = model.NumeroTarjeta;
                    compra.Mes = model.Mes;
                    compra.AnioVen = model.AnioVen;
                    indicador = ValidacionesCompra(compra);
                }
                if (String.IsNullOrEmpty(model.CodCcv))
                {//reviso si no hay codigo ccv
                    ModelState.AddModelError("CodCcv", "El campo CCV: es obligatorio.");
                    indicador = false;
                }
            }
            return indicador;
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor")]
        public ActionResult VenderEntrada(VenderEntradaModel model)
        {
            if (Session["CarritoItemVentas"] != null)
            {
                if (validacionVenta(model))
                {
                    int idVenta = 0;
                    DateTime hoy = DateTime.Today;
                    CuentaUsuario cuenta = new CuentaUsuario();
                    using (var context = new inf245netsoft())
                    {
                        try
                        {
                            List<CarritoItem> carrito = (List<CarritoItem>)Session["CarritoItemVentas"];
                            Ventas ve = new Ventas();
                            int cantidadEntradasTotales = carrito.Sum(c => c.cantidad);
                            try
                            {//si es un usuario registrado busco la cuenta y la asigno luego a la venta
                                cuenta = db.CuentaUsuario.Find(model.Dni);
                            }
                            catch (Exception ex)
                            {//si no es un cliente registrado guardo la venta como si fuera anonima
                                cuenta = db.CuentaUsuario.Find(MagicHelpers.AnonimoUniversal);
                            }
                            ve.fecha = DateTime.Now;
                            ve.cantAsientos = cantidadEntradasTotales;
                            //de todas maneras en la venta se registra el nombre, dni y tipo de documento del que esta comprando.
                            ve.cliente = model.Nombre;
                            ve.codDoc = model.Dni;
                            //--
                            ve.Estado = MagicHelpers.Compra;
                            ve.tipoDoc = 1;
                            ve.montoEfectivoSoles = model.Importe;
                            ve.MontoTotalSoles = model.MontoPagar;
                            ve.montoCreditoSoles = model.MontoTar;
                            ve.montoEfectivoSoles = model.MontoEfe;
                            //--vendedor, guardo el correo del vendedor en la venta
                            ve.vendedor = User.Identity.Name;
                            //
                            if (model.MontoTar > 0 && model.MontoEfe == 0)
                            {//para compra solo en tarjeta
                                ve.modalidad = "T";
                            }
                            if (model.MontoEfe > 0 && model.MontoTar > 0)
                            {//mixto
                                ve.modalidad = "M";
                            }
                            if (model.MontoTar == 0)
                            {//solo efectivo
                                ve.modalidad = "E";
                            }
                            db.Ventas.Add(ve);
                            try
                            {
                                db.SaveChanges();
                                idVenta = ve.codVen;
                            }
                            catch (DbEntityValidationException dbEx)
                            {
                                foreach (var validationErrors in dbEx.EntityValidationErrors)
                                {
                                    foreach (var validationError in validationErrors.ValidationErrors)
                                    {
                                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                                validationError.PropertyName,
                                                                validationError.ErrorMessage);
                                    }
                                }
                            }
                            //para cada item del carrito
                            for (int w = 0; w < carrito.Count; w++)
                            {
                                CarritoItem paquete = carrito[w];
                                //zona del evento
                                ZonaEvento zo = db.ZonaEvento.Find(paquete.idZona);
                                //en que perdiodo de venta estamos
                                PeriodoVenta per = db.PeriodoVenta.Where(c => c.codEvento == paquete.idEvento && c.fechaInicio <= hoy && c.fechaFin >= hoy).ToList().First();
                                PrecioEvento pr = db.PrecioEvento.Where(c => c.codZonaEvento == paquete.idZona && c.codPeriodoVenta == per.idPerVent).ToList().First();
                                //la venta x funcion requerida
                                VentasXFuncion vf = new VentasXFuncion();
                                //si ya existe una venta x funcion de esta venta
                                if (db.VentasXFuncion.Any(c => c.codVen == ve.codVen && c.codFuncion == paquete.idFuncion))
                                {
                                    vf = db.VentasXFuncion.Where(c => c.codVen == ve.codVen && c.codFuncion == paquete.idFuncion).First();
                                    vf.cantEntradas += paquete.cantidad;
                                    vf.subtotal += paquete.cantidad * pr.precio;
                                    float? porcDescuento = 0;
                                    if (model.idPromociones[w] != -1)
                                    {
                                        int idPromocion = model.idPromociones[w];
                                        Promociones promocion = db.Promociones.Where(c => c.codPromo == idPromocion && c.codEvento == paquete.idEvento).First();
                                        porcDescuento = promocion.descuento / 100;
                                    }
                                    vf.descuento += (int?)(vf.subtotal * porcDescuento);
                                    vf.total += paquete.cantidad * pr.precio - vf.descuento;
                                }
                                else
                                {
                                    //creo una nueva ventaxfuncion
                                    vf.codVen = ve.codVen;
                                    vf.cantEntradas = paquete.cantidad;
                                    vf.codFuncion = paquete.idFuncion;
                                    vf.Ventas = ve;
                                    vf.Funcion = db.Funcion.Find(paquete.idFuncion);
                                    vf.hanEntregado = false;
                                    float? porcDescuento = 0;
                                    if (model.idPromociones[w] != -1)
                                    {
                                        int idPromocion = model.idPromociones[w];
                                        Promociones promocion = db.Promociones.Where(c => c.codPromo == idPromocion && c.codEvento == paquete.idEvento).First();
                                        porcDescuento = promocion.descuento / 100;
                                    }
                                    vf.subtotal = paquete.cantidad * pr.precio;
                                    vf.descuento = (int?)(vf.subtotal * porcDescuento);
                                    vf.total = vf.subtotal - vf.descuento;
                                    db.VentasXFuncion.Add(vf);
                                }
                                db.SaveChanges();
                                //detalle de venta
                                DetalleVenta dt = new DetalleVenta();
                                dt.cantEntradas = paquete.cantidad;
                                dt.codFuncion = paquete.idFuncion;
                                dt.codPrecE = pr.codPrecioEvento;
                                dt.total = vf.total;
                                dt.entradasDev = 0;
                                dt.descTot = vf.descuento;
                                dt.codVen = vf.codVen;
                                db.DetalleVenta.Add(dt);
                                if (paquete.filas != null && paquete.filas.Count > 0) paquete.tieneAsientos = true;
                                //actualizo el mondo adeudado 
                                Eventos evento = db.Eventos.Find(paquete.idEvento);
                                evento.monto_adeudado += (double)(paquete.cantidad * pr.precio * evento.porccomision / 100 + evento.montoFijoVentaEntrada);
                                db.SaveChanges();
                                //si tengo asientos, actualizo los asientos a ocupado
                                if (paquete.tieneAsientos)
                                {
                                    for (int i = 0; i < paquete.cantidad; i++)
                                    {
                                        int col = paquete.columnas[i];
                                        int fil = paquete.filas[i];
                                        List<Asientos> listasiento = context.Asientos.Where(x => x.codZona == paquete.idZona && x.fila == fil && x.columna == col).ToList();
                                        AsientosXFuncion actAsiento = context.AsientosXFuncion.Find(listasiento.First().codAsiento, paquete.idFuncion);
                                        actAsiento.estado = MagicHelpers.Ocupado;
                                        actAsiento.codDetalleVenta = dt.codDetalleVenta;
                                        actAsiento.PrecioPagado = pr.precio;
                                    }
                                }
                                else
                                {
                                    //si no tiene asientos es una zonax funcion
                                    ZonaxFuncion ZXF = context.ZonaxFuncion.Find(paquete.idFuncion, paquete.idZona);
                                    if (ZXF.cantLibres < paquete.cantidad)
                                    {
                                        //genero una exception para detener la compra?
                                        throw new Exception();
                                    }
                                    else
                                        ZXF.cantLibres -= paquete.cantidad;
                                }
                                if (User.Identity.IsAuthenticated)
                                {//si es u usuario registrado le aumento los puntos que tiene
                                    try
                                    {
                                        CuentaUsuario dbCuenta = db.CuentaUsuario.Find(cuenta.correo);
                                        dbCuenta.puntos += db.Eventos.Find(paquete.idEvento).puntosAlCliente * paquete.cantidad;
                                    }
                                    catch (Exception ex)
                                    {
                                        Ventas remover = db.Ventas.Find(idVenta);
                                        db.Ventas.Remove(remover);
                                    }

                                }
                            }

                            db.SaveChanges();

                            context.SaveChanges();
                        }
                        catch (OptimisticConcurrencyException ex)
                        {
                            //hubo un problema con la compra, remuevo el item
                            if (idVenta != 0)
                            {
                                Ventas remover = db.Ventas.Find(idVenta);
                                db.Ventas.Remove(remover);
                            }
                            TempData["tipo"] = "alert alert-warning";
                            TempData["message"] = "Error en la compra.";
                            return View(model);
                        }
                    }
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Venta Realizada";
                    //si toda la compra se procesa de manera correcta eliminamos los session
                    Session["CarritoItemVentas"] = null;
                    Session["CarritoVendedor"] = null;
                    //enviamos un correo al cliente que lo compro - no funcionara con un anonimo
                    EmailController.EnviarCorreoCompra(idVenta, cuenta.correo);
                    return RedirectToAction("Index2", "Home");
                }
                //saco el carrito del session
                List<CarritoItem> carrito2 = (List<CarritoItem>)Session["CarritoItemVentas"];
                //lista de bancos
                List<Banco> bancos = db.Banco.ToList();
                ViewBag.Bancos = new SelectList(bancos, "codigo", "nombre");
                //lista de tarjetas
                List<TipoTarjeta> tipoTarjeta = db.TipoTarjeta.ToList();
                ViewBag.TipoTarjeta = new SelectList(tipoTarjeta, "idTipoTar", "nombre");
                List<Promociones> listaPromociones = new List<Promociones>();
                double total = 0;
                double descuento = 0;
                foreach (CarritoItem item in carrito2)
                {
                    total += item.precio;
                    Promociones promocion = CalculaMejorPromocionTarjeta(item.idEvento, bancos.First().codigo, tipoTarjeta.First().idTipoTar);
                    if (promocion == null)
                    {
                        Promociones dummy = new Promociones();
                        dummy.codPromo = -1;
                        listaPromociones.Add(dummy);
                    }
                    else
                    {
                        descuento += item.precio * promocion.descuento.Value / 100;
                        listaPromociones.Add(promocion);
                    }
                }
                ViewBag.Descuento = 0;
                ViewBag.Promociones = listaPromociones;
                ViewBag.Total = total;
                ViewBag.Pagar = total - 0;
                ViewBag.Mes = Fechas.Mes();
                ViewBag.AnVen = Fechas.Anio();
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay items en el carrito.";
            return RedirectToAction("CarritoVentas");
        }
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
            turno.HoraSalida = DateTime.Now;
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

        public ActionResult PagarReserva(string reserva)
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
            List<Pago> listP = db.Pago.Where(c => c.codOrg == idO && c.monto > 0).ToList();
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
            List<Pago> listP = db.Pago.Where(c => c.codOrg == idO && c.monto < 0).ToList();
            Session["Pagos2"] = listP;
            Session["Pendiente2"] = null;
            return RedirectToAction("PagoOrganizador", "Ventas");
        }

        public ActionResult ReporteVentas()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReporteV1a(ReporteModel.ReporteBuscaModel model)
        {

            double to = 0;
            DateTime dt1 = model.fechaI;
            DateTime dt2 = model.fechaF;
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
            return RedirectToAction("ReporteVentas", "Ventas");
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
            List<Pago> listP = db.Pago.Where(c => c.codOrg == org.codOrg && c.monto > 0).ToList();
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
            List<Pago> listP = db.Pago.Where(c => c.codOrg == org.codOrg && c.monto < 0).ToList();
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
                            int codiguitoV = vxf[j].codVen;
                            //filtro
                            Funcion f = db.Funcion.Find(codiguitoF);
                            Ventas vent = db.Ventas.Find(codiguitoV);
                            if (f.estado != "CANCELADO" && f.estado != "POSTERGADO")
                            {
                                vxf.RemoveAt(j);
                                j--;
                            }
                            //logica de que en caso de postergacion no se permita devolver el dinero                                
                            if (f.estado == "POSTERGADO")
                            {
                                Eventos ev = db.Eventos.Find(f.codEvento);
                                if (!ev.devolverPostergacion)
                                {
                                    vxf.RemoveAt(j);
                                    j--;
                                }
                                else
                                {//sí se puede devolver
                                    //DateTime fPost = (DateTime)f.fechaPostergado;
                                    if (f.fechaPostergado != null)
                                    {
                                        if (DateTime.Compare((DateTime)vent.fecha, (DateTime)f.fechaPostergado) > 0)
                                        {//se realizo la compra fuera del rango en que se puede devolver
                                            vxf.RemoveAt(j);
                                            j--;
                                        }
                                    }

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
                        dv.codFuncion == codiguitoF && dv.cantEntradas != 0).ToList();
                        //dv.cantEntradas!=0 en caso se haya realizado una devolucion de un dv y no de la v total

                        for (int j = 0; j < detVen.Count; j++)
                        {
                            //se llena la lista de devoluciones por cada detalle de venta
                            DevolucionModel d = new DevolucionModel();
                            d.codDev = detVen[j].codDetalleVenta;
                            d.numDoc = int.Parse(doc);
                            if (usuario.Count == 0)
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
            DetalleVenta dv = (DetalleVenta)Session["DetalleVenta"];

            Ventas v = db.Ventas.Find(dv.codVen);
            //Ventas v = (Ventas)Session["VentasDev"];
            v.cantAsientos -= (int)dv.cantEntradas;
            v.MontoTotalSoles -= (double)dv.total;
            if (v.cantAsientos == 0) v.Estado = "Devuelto";

            Funcion f = db.Funcion.Find(dv.codFuncion);
            Eventos ev = db.Eventos.Find(f.codEvento);
            //Eventos ev = (Eventos)Session["EventoDev"];
            ev.monto_adeudado -= (double)dv.total;

            CuentaUsuario cu = db.CuentaUsuario.Find(v.cliente);
            cu.puntos -= (int)ev.puntosAlCliente * (int)dv.cantEntradas;

            List<AsientosXFuncion> axf = db.AsientosXFuncion.Where(a => a.codFuncion == f.codFuncion && a.codDetalleVenta == dv.codDetalleVenta).ToList();
            //List<AsientosXFuncion> axf = (List<AsientosXFuncion>)Session["ListaAsientos"];
            for (int i = 0; i < axf.Count; i++)
                axf[i].estado = "libre";

            VentasXFuncion vxf = (db.VentasXFuncion.Where(ven => ven.codVen == v.codVen && ven.codFuncion == f.codFuncion).ToList())[0];
            //VentasXFuncion vxf = (VentasXFuncion)Session["VentaXFunDev"];
            vxf.cantEntradas -= (int)dv.cantEntradas;

            PrecioEvento pe = db.PrecioEvento.Find(dv.codPrecE);
            ZonaEvento ze = db.ZonaEvento.Find(pe.codZonaEvento);
            if (!ze.tieneAsientos) ze.tieneAsientos = true;

            ZonaxFuncion zxf = (db.ZonaxFuncion.Where(z => z.codFuncion == dv.codFuncion && z.codZona == ze.codZona).ToList())[0];
            zxf.cantLibres += (int)dv.cantEntradas;

            DetalleVenta dvAux = db.DetalleVenta.Find(dv.codDetalleVenta);
            dvAux.entradasDev = dvAux.cantEntradas;
            dvAux.cantEntradas = 0;
            /*Session["DetalleVenta"]
            Session["VentaXFunDev"]
            Session["VentasDev"]
            Session["ListaAsientos"] = axf;
            Session["BusquedaDev"] = devolucionModel
            Session["FuncionDev"]
            Session["EventoDev"]
            */

            //elimina de la lista de busqueda! 
            List<DevolucionModel> dev = (List<DevolucionModel>)Session["BusquedaDev"];
            for (int i = 0; i < dev.Count; i++)
                if (dev[i].codDev == dv.codDetalleVenta)
                    dev.RemoveAt(i);
            Session["BusquedaDev"] = dev;

            db.SaveChanges();
            return View("Devolucion");
        }

        public ActionResult devolverParcial(string asiento)
        {
            List<AsientosXFuncion> axf = (List<AsientosXFuncion>)Session["ListaAsientos"];
            DetalleVenta dv = (DetalleVenta)Session["DetalleVenta"];
            if (axf.Count != 0) //numerado
            {
                int codAsiento = int.Parse(asiento);

                //DetalleVenta dv = (DetalleVenta)Session["DetalleVenta"];
                AsientosXFuncion axfuncion = db.AsientosXFuncion.Where(a => a.codAsiento == codAsiento && a.codFuncion == dv.codFuncion).First();
                //AsientosXFuncion axfuncion = db.AsientosXFuncion.Find(codAsiento);
                Ventas v = db.Ventas.Find(dv.codVen);
                //Ventas v = (Ventas)Session["VentasDev"];
                v.cantAsientos -= 1;
                v.MontoTotalSoles -= (double)axfuncion.PrecioPagado;
                if (v.cantAsientos == 0) v.Estado = "Devuelto";
                //v.Estado = "DevueltoParcial"; por evaluar!!!!!!!

                Funcion f = db.Funcion.Find(dv.codFuncion);
                Eventos ev = db.Eventos.Find(f.codEvento);
                //Eventos ev = (Eventos)Session["EventoDev"];
                ev.monto_adeudado -= (double)axfuncion.PrecioPagado;

                CuentaUsuario cu = db.CuentaUsuario.Find(v.cliente);
                cu.puntos -= (int)ev.puntosAlCliente;

                //List<AsientosXFuncion> axf = db.AsientosXFuncion.Where(a => a.codFuncion == f.codFuncion && a.codDetalleVenta == dv.codDetalleVenta).ToList();
                //List<AsientosXFuncion> axf = (List<AsientosXFuncion>)Session["ListaAsientos"];
                //for (int i = 0; i < axf.Count; i++)
                //    axf[i].estado = "libre";
                axfuncion.estado = "libre";

                VentasXFuncion vxf = (db.VentasXFuncion.Where(ven => ven.codVen == v.codVen && ven.codFuncion == f.codFuncion).ToList())[0];
                //VentasXFuncion vxf = (VentasXFuncion)Session["VentaXFunDev"];
                vxf.cantEntradas -= 1;

                PrecioEvento pe = db.PrecioEvento.Find(dv.codPrecE);
                ZonaEvento ze = db.ZonaEvento.Find(pe.codZonaEvento);
                if (!ze.tieneAsientos) ze.tieneAsientos = true;

                ZonaxFuncion zxf = (db.ZonaxFuncion.Where(z => z.codFuncion == dv.codFuncion && z.codZona == ze.codZona).ToList())[0];
                zxf.cantLibres += 1;

                DetalleVenta dvAux = db.DetalleVenta.Find(dv.codDetalleVenta);
                dvAux.entradasDev += 1;
                dvAux.cantEntradas -= 1;
                /*Session["DetalleVenta"]
                Session["VentaXFunDev"]
                Session["VentasDev"]
                Session["ListaAsientos"] = axf;
                Session["BusquedaDev"] = devolucionModel
                Session["FuncionDev"]
                Session["EventoDev"]
                */

                //elimina de la lista de busqueda! 

                //List<DevolucionModel> dev = (List<DevolucionModel>)Session["BusquedaDev"];
                for (int i = 0; i < axf.Count; i++)
                    if (axf[i].codAsiento == codAsiento)
                    {
                        axf.RemoveAt(i);
                        break;
                    }
                Session["ListaAsientos"] = axf;

            }
            else // no numerado
            {
                
                //AsientosXFuncion axfuncion = db.AsientosXFuncion.Find(codAsiento);
                PrecioEvento pe = db.PrecioEvento.Find(dv.codPrecE);
                Ventas v = db.Ventas.Find(dv.codVen);
                //Ventas v = (Ventas)Session["VentasDev"];
                v.cantAsientos -= 1;
                v.MontoTotalSoles -= (double)pe.precio;
                if (v.cantAsientos == 0) v.Estado = "Devuelto";
                //v.Estado = "DevueltoParcial"; por evaluar!!!!!!!

                Funcion f = db.Funcion.Find(dv.codFuncion);
                Eventos ev = db.Eventos.Find(f.codEvento);
                //Eventos ev = (Eventos)Session["EventoDev"];
                ev.monto_adeudado -= (double)pe.precio;

                CuentaUsuario cu = db.CuentaUsuario.Find(v.cliente);
                cu.puntos -= (int)ev.puntosAlCliente;

                //List<AsientosXFuncion> axf = db.AsientosXFuncion.Where(a => a.codFuncion == f.codFuncion && a.codDetalleVenta == dv.codDetalleVenta).ToList();
                //List<AsientosXFuncion> axf = (List<AsientosXFuncion>)Session["ListaAsientos"];
                //for (int i = 0; i < axf.Count; i++)
                //    axf[i].estado = "libre";
                //axfuncion.estado = "libre";

                VentasXFuncion vxf = (db.VentasXFuncion.Where(ven => ven.codVen == v.codVen && ven.codFuncion == f.codFuncion).ToList())[0];
                //VentasXFuncion vxf = (VentasXFuncion)Session["VentaXFunDev"];
                vxf.cantEntradas -= 1;
                ;
                ZonaEvento ze = db.ZonaEvento.Find(pe.codZonaEvento);
                if (!ze.tieneAsientos) ze.tieneAsientos = true;

                ZonaxFuncion zxf = (db.ZonaxFuncion.Where(z => z.codFuncion == dv.codFuncion && z.codZona == ze.codZona).ToList())[0];
                zxf.cantLibres += 1;

                DetalleVenta dvAux = db.DetalleVenta.Find(dv.codDetalleVenta);
                dvAux.entradasDev += 1;
                dvAux.cantEntradas -= 1;
                /*Session["DetalleVenta"]
                Session["VentaXFunDev"]
                Session["VentasDev"]
                Session["ListaAsientos"] = axf;
                Session["BusquedaDev"] = devolucionModel
                Session["FuncionDev"]
                Session["EventoDev"]
                */

                /*
                List<DevolucionModel> dev = (List<DevolucionModel>)Session["BusquedaDev"];
                for (int i = 0; i < dev.Count; i++)
                    if (dev[i].codDev == dv.codDetalleVenta)
                        dev.RemoveAt(i);
                Session["BusquedaDev"] = dev;*/
            }
            /*Session["DetalleVenta"]
            Session["VentaXFunDev"]
            Session["VentasDev"]
            Session["ListaAsientos"] = axf;
            Session["BusquedaDev"] = devolucionModel
            Session["FuncionDev"]
            Session["EventoDev"]
            Session["ZonaEventoDev"]
            Session["AsientosDev"]*/
            
            List<DevolucionModel> devolMod = (List<DevolucionModel>)Session["BusquedaDev"];
            int j;
            for (j = 0; j < devolMod.Count;j++ )
            {
                if (devolMod[j].codDev == dv.codDetalleVenta) break;                                
            }
            devolMod[j].cantAsientos -= 1;
            Session["BusquedaDev"] = devolMod;

            db.SaveChanges();
            return View("VerDetalle");
        }

        public ActionResult VerDetalle(string detVen)
        {
            //Session["Bus"] = null;
            int id;
            if (detVen == null)
            {
                DetalleVenta dVen = (DetalleVenta)Session["DetalleVenta"];
                id = dVen.codDetalleVenta;
            }
            else id = int.Parse(detVen);
            ViewBag.id = id;
            TempData["codigoDet"] = id;

            DetalleVenta detalleVen = db.DetalleVenta.Find(id);
            Session["DetalleVenta"] = detalleVen;
            VentasXFuncion venxf = db.VentasXFuncion.Where(v => v.codFuncion == detalleVen.codFuncion && v.codVen == detalleVen.codVen).ToList()[0];
            Session["VentaXFunDev"] = venxf;
            Ventas ven = db.Ventas.Find(venxf.codVen);
            Session["VentasDev"] = ven;

            List<AsientosXFuncion> axf = db.AsientosXFuncion.Where(a => a.codFuncion == detalleVen.codFuncion && a.codDetalleVenta == detalleVen.codDetalleVenta && a.estado=="OCUPADO").ToList();
            Session["ListaAsientos"] = axf;

            List<Asientos> asientos = null;
            if (axf.Count != 0)
            {
                //if(axf!=null){
                asientos = new List<Asientos>();
                for (int i = 0; i < axf.Count; i++)
                {
                    Asientos asi = db.Asientos.Find(axf[i].codAsiento);
                    asientos.Add(asi);
                }
            }
            Session["AsientosDev"] = asientos;


            Funcion funDev = db.Funcion.Find(detalleVen.codFuncion);
            Session["FuncionDev"] = funDev;
            Eventos eventoDev = db.Eventos.Find(funDev.codEvento);
            Session["EventoDev"] = eventoDev;

            PrecioEvento pe = db.PrecioEvento.Find(detalleVen.codPrecE);
            ZonaEvento ze = db.ZonaEvento.Find(pe.codZonaEvento);

            Session["ZonaEventoDev"] = ze;
            if (detalleVen.cantEntradas == 0) {//caso de devolucion parcial.
                List<DevolucionModel> dev = (List<DevolucionModel>)Session["BusquedaDev"];
                for (int i = 0; i < dev.Count; i++)
                    if (dev[i].codDev == id)
                        dev.RemoveAt(i);
                Session["BusquedaDev"] = dev;
                return View("Devolucion");
            }
            else  return View("VerDetalle");
        }

    }
}