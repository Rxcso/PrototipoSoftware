using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using PagedList;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Core;

namespace WebApplication4.Controllers
{

    public partial class EventoController : Controller
    {
        inf245netsoft db = new inf245netsoft();
        const int maximoPaginas = 2;
        const int cantMax = 6;

        public int BuscarEntradasLeQuedan(string name, int idFuncion)
        {
            CuentaUsuario cuenta = db.CuentaUsuario.Find(name);
            int tipoDoc = (int)cuenta.tipoDoc;
            string doc = cuenta.codDoc;

            List<VentasXFuncion> listVXF = db.VentasXFuncion.Where(x => x.codFuncion == idFuncion).ToList();

            Funcion funcion = db.Funcion.Find(idFuncion);
            int limEntradas = funcion.Eventos.maxReservas;
            int actualEntradas = 0;
            foreach (VentasXFuncion VXF in listVXF)
            {
                if (VXF.Ventas.codDoc.CompareTo(doc) == 0 && VXF.Ventas.tipoDoc == tipoDoc)
                {
                    actualEntradas += VXF.cantEntradas;
                }
            }
            return limEntradas - actualEntradas;
        }

        public int BuscaEntradasQueQuedan(int idFuncion, int idZona)
        {
            ZonaxFuncion zonaxfuncion = db.ZonaxFuncion.Where(c => c.codFuncion == idFuncion && c.codZona == idZona).First();
            return zonaxfuncion.cantLibres;
        }

        public string reservaAsientos(string name, PaqueteEntradas paquete)
        {
            //Si todo esta bien se devuelve OK
            //Que significa todo Ok?
            //Primero busca cuantas entradas mas puede comprar/reservar esta persona para esa funcion
            //Si supera el limite fue ps
            int idVenta = 0;
            try
            {
                int quedan = BuscarEntradasLeQuedan(name, paquete.idFuncion);

                if (quedan == 0)
                {
                    return "Ya no quedan reservas/compras disponibles para el evento";
                }

                if (quedan < paquete.cantEntradas)
                {
                    if (quedan > 0)
                    {
                        return "No se pudo realizar la reserva, solo puede reservar hasta  " + quedan + " entradas.";
                    }
                    return "Ya no puede reservar más entradas para este evento.";
                }
                //Luego se hace la reserva de esto, 
                //Establecer sincronia es lo mas complicado
                //Apenas se guarde la reserva todo estara consumado XD 
                //Eso es todo

                using (var context = new inf245netsoft())
                {
                    try
                    {
                        Ventas ve = new Ventas();
                        //Ventas vel = context.Ventas.ToList().Last();
                        DateTime hoy = DateTime.Now.Date;
                        ZonaEvento zo = context.ZonaEvento.Find(paquete.idZona);
                        PeriodoVenta per = context.PeriodoVenta.Where(c => c.codEvento == paquete.idEvento && c.fechaInicio <= hoy && c.fechaFin >= hoy).ToList().First();
                        PrecioEvento pr = context.PrecioEvento.Where(c => c.codZonaEvento == paquete.idZona && c.codPeriodoVenta == per.idPerVent).ToList().First();
                        //ve.codVen = vel.codVen + 1;
                        CuentaUsuario cuenta = context.CuentaUsuario.Find(User.Identity.Name);
                        ve.fecha = DateTime.Now;
                        ve.cantAsientos = paquete.cantEntradas;
                        ve.cliente = cuenta.usuario;
                        ve.codDoc = cuenta.codDoc;
                        ve.Estado = "Reservado";
                        ve.tipoDoc = cuenta.tipoDoc;
                        ve.montoEfectivoSoles = paquete.cantEntradas * pr.precio;
                        ve.MontoTotalSoles = paquete.cantEntradas * pr.precio;
                        context.Ventas.Add(ve);
                        context.SaveChanges();
                        VentasXFuncion vf = new VentasXFuncion();
                        vf.hanEntregado = false;
                        vf.codVen = ve.codVen;
                        idVenta = ve.codVen;
                        vf.cantEntradas = paquete.cantEntradas;
                        vf.codFuncion = paquete.idFuncion;
                        vf.Ventas = ve;
                        vf.Funcion = context.Funcion.Find(paquete.idFuncion);
                        vf.descuento = 0;
                        vf.subtotal = paquete.cantEntradas * pr.precio;
                        vf.total = paquete.cantEntradas * pr.precio;
                        vf.hanEntregado = false;
                        context.VentasXFuncion.Add(vf);
                        //context.SaveChanges();
                        DetalleVenta dt = new DetalleVenta();
                        dt.cantEntradas = paquete.cantEntradas;
                        //dt.codDetalleVenta = context.DetalleVenta.ToList().Last().codDetalleVenta + 1;
                        dt.codFuncion = paquete.idFuncion;
                        dt.codPrecE = pr.codPrecioEvento;
                        dt.total = paquete.cantEntradas * pr.precio;
                        dt.entradasDev = 0;
                        dt.descTot = 0;
                        dt.codVen = vf.codVen;
                        context.DetalleVenta.Add(dt);
                        if (paquete.filas != null && paquete.filas.Count > 0) paquete.tieneAsientos = true;
                        context.SaveChanges();
                        if (paquete.tieneAsientos)
                        {
                            for (int i = 0; i < paquete.cantEntradas; i++)
                            {
                                int col = paquete.columnas[i];
                                int fil = paquete.filas[i];
                                List<Asientos> listasiento = context.Asientos.Where(x => x.codZona == paquete.idZona && x.fila == fil && x.columna == col).ToList();
                                AsientosXFuncion actAsiento = context.AsientosXFuncion.Find(listasiento.First().codAsiento, paquete.idFuncion);
                                if (actAsiento.estado == "libre")
                                {
                                    throw new OptimisticConcurrencyException();
                                }
                                actAsiento.estado = "OCUPADO";
                                actAsiento.codDetalleVenta = dt.codDetalleVenta;
                                actAsiento.PrecioPagado = pr.precio;
                            }
                        }
                        else
                        {
                            ZonaxFuncion ZXF = context.ZonaxFuncion.Find(paquete.idFuncion, paquete.idZona);
                            if (ZXF.cantLibres < paquete.cantEntradas)
                            {
                                context.Dispose();
                                Ventas ventaAct = db.Ventas.Find(idVenta);
                                db.Ventas.Remove(ventaAct);
                                db.SaveChanges();
                                return "No hay suficientes entradas";
                            }
                            ZXF.cantLibres -= paquete.cantEntradas;
                        }
                        context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        context.Dispose();
                        Ventas ventaAct = db.Ventas.Find(idVenta);
                        if (ventaAct != null) db.Ventas.Remove(ventaAct);
                        db.SaveChanges();
                        return "No se pudieron reservar los asientos, alguien más ya lo hizo.";
                    }
                }
            }
            catch (Exception ex)
            {
                Ventas ventaAct = db.Ventas.Find(idVenta);
                if (ventaAct != null) db.Ventas.Remove(ventaAct);
                db.SaveChanges();
                return "Ocurrio un error inesperado.";
            }
            //Funciones Utilitarias necesarias
            //BuscarEntradasLeQuedan( User , Funcion )
            EmailController.EnviarCorreoReserva(idVenta, User.Identity.Name);
            return "Ok";
        }

        public JsonResult LimpiaR1()
        {
            Session["ReporteEventos"] = null;
            return Json("Reporte Limpio", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReporteEvento1(string fd, string fh)
        {

            double to = 0;
            int codFuncion;
            int cantLibres;
            int cantTotal;
            DateTime dt1 = DateTime.Parse(fd);
            DateTime dt2 = DateTime.Now;
            if (fh != null && fh != "")
            {
                dt2 = DateTime.Parse(fh);
            }
            Session["FechaREI"] = dt1;
            Session["FechaREF"] = dt2;
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            DateTime di = dt1;
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            List<ReporteEventosModel> lr = new List<ReporteEventosModel>();

            List<Eventos> lv1 = db.Eventos.Where(c => c.fecha_inicio >= dt1.Date).ToList();
            List<Eventos> lv = lv1.Where(c => c.fecha_fin <= dt2.Date).ToList();
            List<Funcion> lf;
            List<ZonaxFuncion> lzf;
            List<ZonaEvento> lze;
            for (int i = 0; i < lv.Count; i++)
            {
                ReporteEventosModel r = new ReporteEventosModel();
                r.codigoEvento = lv[i].codigo;
                r.nombreEvento = lv[i].nombre;
                int valorID = (int)lv[i].idOrganizador;
                Organizador queryO = db.Organizador.Where(c => c.codOrg == valorID).First();
                r.nombreOrganizador = queryO.nombOrg;

                valorID = (int)lv[i].idRegion;
                Region querR = db.Region.Where(c => c.idRegion == (int)valorID).First();
                r.region = querR.nombre;


                if (lv[i].idLocal != null)
                {
                    valorID = (int)lv[i].idLocal;
                    Local querL = db.Local.Where(c => c.codLocal == valorID).First();
                    r.local = querL.ubicacion;
                }

                else
                {
                    r.local = lv[i].direccion;
                }

                valorID = (int)lv[i].codigo;
                lf = db.Funcion.Where(c => c.codEvento == valorID).ToList();
                DateTime fecha, hora;

                cantTotal = 0;
                lze = db.ZonaEvento.Where(c => c.codEvento == valorID).ToList();
                for (int h = 0; h < lze.Count; h++)
                {
                    cantTotal = cantTotal + lze[h].aforo;
                }

                for (int j = 0; j < lf.Count; j++)
                {
                    fecha = (DateTime)lf[j].fecha;
                    r.fechaFuncion = fecha.ToString("dd-MM-yyyy");
                    hora = (DateTime)lf[j].horaIni;
                    r.horaFuncion = hora.ToString("hh:mm tt");
                    //cantidades de entradas disponibles
                    cantLibres = 0;
                    codFuncion = (int)lf[j].codFuncion;
                    lzf = db.ZonaxFuncion.Where(c => c.codFuncion == codFuncion).ToList();
                    for (int k = 0; k < lzf.Count; k++)
                    {
                        cantLibres = cantLibres + lzf[k].cantLibres;
                    }
                    r.entradasDisponibles = cantLibres;
                    //cantidades de entradas vendidas

                    r.entradasVendidas = cantTotal - cantLibres;
                    //estado de la funcion
                    r.EstadoFunción = lf[j].estado;
                    lr.Add(r);

                    r = new ReporteEventosModel();
                    r.codigoEvento = lv[i].codigo;
                    r.nombreEvento = lv[i].nombre;
                    valorID = (int)lv[i].idOrganizador;
                    queryO = db.Organizador.Where(c => c.codOrg == valorID).First();
                    r.nombreOrganizador = queryO.nombOrg;

                    valorID = (int)lv[i].idRegion;
                    querR = db.Region.Where(c => c.idRegion == (int)valorID).First();
                    r.region = querR.nombre;
                    if (lv[i].idLocal != null)
                    {
                        valorID = (int)lv[i].idLocal;
                        Local querL = db.Local.Where(c => c.codLocal == valorID).First();
                        r.local = querL.ubicacion;
                    }

                    else
                    {
                        r.local = lv[i].direccion;
                    }
                }

            }

            Session["ReporteEventos"] = lr;
            return Json("Reporte Generado", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ReporteEvento()
        {
            return View();
        }

        public ActionResult Index(string nombre, string orden, DateTime? fech_ini, DateTime? fech_fin, int? idCategoria, int? idSubCat, int? idRegion, int? page)
        {
            //cada vez que se va al index limpio los session involucrados en la creacion o modificacion
            Session["IdEventoModificado"] = null;
            Session["IdEventoCreado"] = null;
            //----//

            ViewBag.nombre = nombre;
            ViewBag.fech_ini = fech_ini;
            ViewBag.fech_fin = fech_fin;
            ViewBag.idCategoria = idCategoria;

            ViewBag.idSubCat = idSubCat;
            ViewBag.idRegion = idRegion;

            var lista = from obj in db.Eventos
                        select obj;

            if (!String.IsNullOrEmpty(nombre))
            {
                lista = lista.Where(s => s.nombre.Contains(nombre));
            }

            if (fech_ini.HasValue)
            {

                lista = lista.Where(c => c.fecha_inicio >= fech_ini);
            }

            if (fech_fin.HasValue)
            {
                lista = lista.Where(c => c.fecha_inicio <= fech_fin);
            }

            if (idCategoria.HasValue)
            {
                lista = lista.Where(c => c.idCategoria == idCategoria);
            }

            if (idSubCat.HasValue)
            {
                lista = lista.Where(c => c.idSubcategoria == idSubCat);
            }

            if (idRegion.HasValue)
            {
                lista = lista.Where(c => c.idRegion == idRegion);
            }
            switch (orden)
            {
                default:
                    lista = lista.OrderByDescending(s => s.codigo);
                    break;
            }

            var categorias = db.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.categorias = new SelectList(categorias, "idCategoria", "nombre");
            var departamentos = db.Region.AsNoTracking().Where(c => c.idRegPadre == null);
            ViewBag.departamentos = new SelectList(departamentos, "idRegion", "nombre");
            List<Region> listProv = new List<Region>();
            List<Categoria> listSubCat = new List<Categoria>();

            ViewBag.distritos = new SelectList(listProv, "idProv", "nombre");
            ViewBag.subcategorias = new SelectList(listSubCat, "idSubcat", "nombre");
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult ModificarEvento(int? evento)
        {
            if (evento == null) return RedirectToAction("Index");
            Eventos EventoUp = db.Eventos.Find(evento);
            //verifico que no haya empezado la venta aun
            if (EventoUp.fecha_inicio <= DateTime.Today)
            {
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "El evento ya empezó. No se puede modificar. En caso desee modificarlo consulte con un administrador.";
                return RedirectToAction("Index");
            }
            //Carga de datos
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias && c.activo == 1).ToList();
            DatosGeneralesModel model = new DatosGeneralesModel();
            ViewBag.MensajeExtra = "Modificación de Evento";
            //Buscamos el evento
            Session["IdEventoModificado"] = EventoUp.codigo;
            model.descripcion = EventoUp.descripcion;
            model.Direccion = EventoUp.direccion;
            model.idCategoria = (int)EventoUp.idCategoria;
            model.idSubCat = (EventoUp.idSubcategoria == null) ? 0 : (int)EventoUp.idSubcategoria;
            model.idOrganizador = (int)EventoUp.idOrganizador;
            model.idRegion = (int)EventoUp.idRegion;
            model.idProv = (EventoUp.idProvincia == null) ? 0 : (int)EventoUp.idProvincia;
            model.Local = (EventoUp.idLocal == null) ? 0 : (int)EventoUp.idLocal;
            model.nombre = EventoUp.nombre;
            //categorias
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre", model.idCategoria);
            //subcategoria
            List<Categoria> listSubCat = db.Categoria.Where(c => c.idCatPadre == model.idCategoria && c.activo == 1).ToList();
            ViewBag.SubID = new SelectList(listSubCat, "idCategoria", "nombre", model.idSubCat);
            //departamento
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre", model.idRegion);
            //provincia
            List<Region> listProv = db.Region.Where(c => c.idRegPadre == model.idRegion).ToList();
            ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre", model.idProv);
            //organizador
            ViewBag.NombreOrganizador = db.Organizador.Find(model.idOrganizador).nombOrg;
            //nombre del local si es que existe
            Local p = new Local();
            try
            {
                p = db.Local.Find(model.Local);
                ViewBag.NombreLocal = p.ubicacion;
            }
            catch (Exception ex)
            {
                ViewBag.NombreLocal = "";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ModificarEvento(DatosGeneralesModel model)
        {
            int idEvento = 0;
            ViewBag.MensajeExtra = "Modificación de Evento";
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = db.Region.Where(c => c.idRegPadre == model.idRegion).ToList();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre");
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias && c.activo == 1).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            //subcategoria
            List<Categoria> listSubCat = db.Categoria.Where(c => c.idCatPadre == model.idCategoria && c.activo == 1).ToList();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idCategoria", "nombre");
            //organizador
            ViewBag.NombreOrganizador = db.Organizador.Find(model.idOrganizador).nombOrg;
            //nombre del local si es que existe
            Local p = new Local();
            try
            {
                p = db.Local.Find(model.Local);
                ViewBag.NombreLocal = p.ubicacion;
            }
            catch (Exception ex)
            {
                ViewBag.NombreLocal = "";
            }
            if (ModelState.IsValid)
            {
                if (int.TryParse(Session["IdEventoModificado"].ToString(), out idEvento))
                {
                    Eventos modificado = db.Eventos.Find(idEvento);
                    modificado.descripcion = model.descripcion;
                    modificado.direccion = model.Direccion;
                    modificado.idCategoria = model.idCategoria;
                    modificado.idOrganizador = model.idOrganizador;
                    modificado.idProvincia = model.idProv;
                    modificado.idRegion = model.idRegion;
                    modificado.idSubcategoria = model.idSubCat;
                    modificado.idLocal = model.Local;
                    modificado.nombre = model.nombre;
                    modificado.fechaUltModificacion = DateTime.Today;
                    modificado.estado = "Modificado";
                    db.SaveChanges();
                    return RedirectToAction("BloquesTiempoVenta");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DatosGenerales()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias && c.activo == 1).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = new List<Categoria>();
            DatosGeneralesModel model = new DatosGeneralesModel();
            ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idCategoria", "nombre");
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.MensajeExtra = "Nuevo Evento: Datos Generales";
            ViewBag.NombreOrganizador = "";
            ViewBag.NombreLocal = "";
            return View(model);
        }

        [HttpPost]
        public ActionResult DatosGenerales(DatosGeneralesModel model)
        {
            if (ModelState.IsValid && model.idOrganizador != 0)
            {
                Eventos evento = new Eventos();
                evento.nombre = model.nombre;
                evento.idOrganizador = model.idOrganizador;
                evento.idCategoria = model.idCategoria;
                evento.idSubcategoria = (model.idSubCat == 0) ? 0 : model.idSubCat;
                evento.idLocal = (model.Local == 0) ? 0 : model.Local;
                evento.direccion = string.IsNullOrEmpty(model.Direccion) ? "" : model.Direccion;
                evento.idRegion = (model.idRegion == 0) ? 0 : model.idRegion;
                evento.idProvincia = (model.idProv == 0) ? 0 : model.idProv;
                evento.descripcion = string.IsNullOrEmpty(model.descripcion) ? "" : model.descripcion;
                evento.fechaRegistro = DateTime.Today;
                evento.estado = "Creado";
                evento.monto_adeudado = 0;
                evento.monto_transferir = 0;
                evento.tieneBoletoElectronico = false;
                evento.permiteReserva = false;
                evento.puntosAlCliente = 0;
                evento.hanPostergado = false;
                evento.hanCancelado = false;
                evento.maxReservas = 0;
                //evento.ImagenDestacado = MagicHelpers.NuevoEvento;
                db.Eventos.Add(evento);
                db.SaveChanges();
                int id = evento.codigo;
                Session["IdEventoCreado"] = id;
                return RedirectToAction("BloquesTiempoVenta");
            }
            if (model.idOrganizador == 0)
                ModelState.AddModelError("idOrganizador", "El evento debe tener un organizador");
            ViewBag.MensajeExtra = "Revise los errores.";
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = db.Region.Where(c => c.idRegPadre == model.idRegion).ToList();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idRegion", "nombre");
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias && c.activo == 1).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = db.Categoria.Where(c => c.idCatPadre == model.idCategoria && c.activo == 1).ToList();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idCategoria", "nombre");
            try
            {
                ViewBag.NombreOrganizador = db.Organizador.Find(model.idOrganizador).nombOrg;
            }
            catch (Exception ex)
            {
                ViewBag.NombreOrganizador = "";
            }
            //nombre del local si es que existe
            Local p = new Local();
            try
            {
                p = db.Local.Find(model.Local);
                ViewBag.NombreLocal = p.ubicacion;
            }
            catch (Exception ex)
            {
                ViewBag.NombreLocal = "";
            }
            return View(model);
        }

        public ActionResult BloquesTiempoVenta()
        {
            List<BloqueDeTiempoModel> listaResultado = new List<BloqueDeTiempoModel>();
            //IdEventoCreado
            if (Session["IdEventoModificado"] != null)
            {
                int idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                BloqueTiempoListModel model = new BloqueTiempoListModel();
                model.esCorrecto = true;
                List<PeriodoVenta> listaPer = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                listaResultado = new List<BloqueDeTiempoModel>();
                foreach (PeriodoVenta pventa in listaPer)
                {
                    BloqueDeTiempoModel bloque = new BloqueDeTiempoModel();
                    bloque.fechaFin = (DateTime)pventa.fechaFin;
                    bloque.fechaInicio = (DateTime)pventa.fechaInicio;
                    listaResultado.Add(bloque);
                }
                ViewBag.Resultados = listaResultado;
                return View();
            }
            else
            {
                //Verifico si hay un evento creado en progeso
                if (Session["IdEventoCreado"] != null) return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación";
            return RedirectToAction("Index");
        }

        private void FiltraBloques(List<BloqueDeTiempoModel> lista, int idEvento)
        {
            //agregar nuevos bloques de venta
            List<PeriodoVenta> agregar = new List<PeriodoVenta>();
            //no agregar los ya existentes
            List<PeriodoVenta> existentes = new List<PeriodoVenta>();
            //busco todos los periodos de venta del evento
            List<PeriodoVenta> periodo = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
            //Por cada bloque nuevo, verifico si tengo que agregarlo o si ya existe
            for (int i = 0; i < lista.Count; i++)
            {
                PeriodoVenta pventa = new PeriodoVenta();
                try
                {
                    pventa = periodo.Where(c => c.fechaInicio == lista[i].fechaInicio && c.fechaFin == lista[i].fechaFin).First();
                }
                catch (Exception ex)
                {
                    pventa = null;
                }

                //si no exite lo tengo que agregar
                if (pventa == null)
                {
                    PeriodoVenta agregarPV = new PeriodoVenta();
                    agregarPV.fechaFin = lista[i].fechaFin;
                    agregarPV.fechaInicio = lista[i].fechaInicio;
                    agregarPV.codEvento = idEvento;
                    agregar.Add(agregarPV);
                }

                //si existe, no le tengo que hacer nada
                existentes.Add(pventa);
            }
            for (int i = 0; i < agregar.Count; i++)
            {
                //los agrego a la base de datos
                db.PeriodoVenta.Add(agregar[i]);
                db.SaveChanges();
            }
            //remuevo los que ya existen
            for (int i = 0; i < existentes.Count; i++)
                periodo.Remove(existentes[i]);
            //Al final me quedan los que debo eliminar
            for (int i = 0; i < periodo.Count; i++)
            {
                //busco las tarfias asociadas al bloque de venta
                int idPeriodo = periodo[i].idPerVent;
                List<PrecioEvento> tarifas = db.PrecioEvento.Where(c => c.codPeriodoVenta == idPeriodo).ToList();
                //elimino cada una de las tarifas
                for (int j = 0; j < tarifas.Count; j++)
                {
                    db.PrecioEvento.Remove(tarifas[j]);
                    db.SaveChanges();
                }
                //elimino el bloque
                db.PeriodoVenta.Remove(periodo[i]);
                db.SaveChanges();
            }
        }

        private void ObtenerFechaInicio(List<BloqueDeTiempoModel> bloques, int idEvento)
        {
            List<DateTime> inicio = new List<DateTime>();
            foreach (BloqueDeTiempoModel bloque in bloques)
            {
                inicio.Add(bloque.fechaInicio);
            }
            inicio.Sort((a, b) => a.CompareTo(b));
            DateTime fechaInicio = inicio.First();
            Eventos evento = db.Eventos.Find(idEvento);
            evento.fecha_inicio = fechaInicio;
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult BloquesTiempoVenta(BloqueTiempoListModel model)
        {
            int idEvento = 0;
            List<BloqueDeTiempoModel> listaVerificacion = null;
            if (Session["IdEventoCreado"] != null || Session["IdEventoModificado"] != null)
            {
                if (Session["IdEventoCreado"] != null)
                    idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                if (Session["IdEventoModificado"] != null)
                    idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                listaVerificacion = Validaciones.ValidarBloquesDeTiempoDeVenta(model);
                if (model.esCorrecto)
                {
                    ObtenerFechaInicio(listaVerificacion, idEvento);
                    if (Session["IdEventoModificado"] != null)
                    {
                        FiltraBloques(listaVerificacion, idEvento);
                        return RedirectToAction("Funciones");
                    }
                    for (int i = 0; i < listaVerificacion.Count; i++)
                    {
                        PeriodoVenta periodoVenta = new PeriodoVenta();
                        periodoVenta.fechaInicio = listaVerificacion[i].fechaInicio;
                        periodoVenta.fechaFin = listaVerificacion[i].fechaFin;
                        periodoVenta.codEvento = idEvento;
                        db.PeriodoVenta.Add(periodoVenta);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Funciones");
                }
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "Hay bloques de tiempo que se cruzan. Verifique nuevamente.";
                ViewBag.Resultados = listaVerificacion;
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }

        public ActionResult Funciones()
        {
            ViewBag.MensajeError = "";
            List<FuncionesModel> listaVerificacion = new List<FuncionesModel>();
            int idEvento = 0;
            DateTime fechaInicio = new DateTime();
            if (Session["IdEventoCreado"] != null || Session["IdEventoModificado"] != null)
            {
                if (Session["IdEventoCreado"] != null)
                {
                    idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                    //fecha inicio del evento
                    fechaInicio = (DateTime)db.Eventos.Where(c => c.codigo == idEvento).First().fecha_inicio;
                    ViewBag.FechaInicio = fechaInicio;
                    return View();
                }
                idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                fechaInicio = (DateTime)db.Eventos.Where(c => c.codigo == idEvento).First().fecha_inicio;
                ViewBag.FechaInicio = fechaInicio;
                //busco todos las funciones del evento
                List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == idEvento).ToList();
                //preparo mi model
                List<FuncionesModel> funcionesModel = new List<FuncionesModel>();
                foreach (Funcion func in funciones)
                {
                    FuncionesModel model = new FuncionesModel();
                    model.fechaFuncion = (DateTime)func.fecha;
                    model.horaInicio = (DateTime)func.horaIni;
                    funcionesModel.Add(model);
                }
                ViewBag.Resultados = funcionesModel;
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }

        private void FiltrarFunciones(List<FuncionesModel> lista, int idEvento)
        {
            //agregar nuevas funciones
            List<Funcion> agregar = new List<Funcion>();
            //no agregar los ya existentes
            List<Funcion> existentes = new List<Funcion>();
            //busco todas las funciones del evento
            List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == idEvento).ToList();
            //Por cada bloque nuevo, verifico si tengo que agregarlo o si ya existe
            for (int i = 0; i < lista.Count; i++)
            {
                Funcion funcion = new Funcion();
                try
                {
                    funcion = funciones.Where(c => ((DateTime)c.fecha).Date == lista[i].fechaFuncion.Date && TimeSpan.Compare(((DateTime)c.horaIni).TimeOfDay, lista[i].horaInicio.TimeOfDay) == 0).First();
                }
                catch (Exception ex)
                {
                    funcion = null;
                }

                //si no exite lo tengo que agregar
                if (funcion == null)
                {
                    Funcion fAgr = new Funcion();
                    fAgr.fecha = lista[i].fechaFuncion;
                    fAgr.horaIni = lista[i].horaInicio;
                    fAgr.codEvento = idEvento;
                    fAgr.estado = "ACTIVO";
                    agregar.Add(fAgr);
                }
                //si existe, no le tengo que hacer nada
                existentes.Add(funcion);
            }
            for (int i = 0; i < agregar.Count; i++)
            {
                //los agrego a la base de datos
                db.Funcion.Add(agregar[i]);
                db.SaveChanges();
            }
            //remuevo los que ya existen
            for (int i = 0; i < existentes.Count; i++)
                funciones.Remove(existentes[i]);
            //Al final me quedan los que debo eliminar
            for (int i = 0; i < funciones.Count; i++)
            {
                db.Funcion.Remove(funciones[i]);
                db.SaveChanges();
            }
        }

        private void ObtenerFechaFin(int idEvento)
        {
            try
            {
                //cambio solo funciones no canceladas
                List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == idEvento && c.estado.CompareTo("CANCELADO") != 0).ToList();
                List<DateTime> fin = new List<DateTime>();
                foreach (Funcion funcion in funciones)
                {
                    fin.Add((DateTime)funcion.fecha);
                }
                fin.Sort((a, b) => a.CompareTo(b));
                DateTime fechaFin = fin.Last();
                Eventos evento = db.Eventos.Find(idEvento);
                evento.fecha_fin = fechaFin;
                db.SaveChanges();
            }
            catch
            {
                Eventos evento = db.Eventos.Find(idEvento);
                evento.fecha_fin = evento.fecha_inicio;
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult Funciones(FuncionesListModel model)
        {
            List<FuncionesModel> listaVerificacion = null;
            int idEvento = 0;
            if (Session["IdEventoCreado"] != null || Session["IdEventoModificado"] != null)
            {
                if (Session["IdEventoCreado"] != null)
                    idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                if (Session["IdEventoModificado"] != null)
                    idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                listaVerificacion = Validaciones.ValidarFunciones(model);
                if (model.esCorrecto)
                {
                    //si solo tiene una funcion, es un evento unico
                    Eventos evento = db.Eventos.Find(idEvento);
                    db.SaveChanges();
                    if (Session["IdEventoModificado"] != null)
                    {
                        FiltrarFunciones(listaVerificacion, idEvento);
                        ObtenerFechaFin(idEvento);
                        return RedirectToAction("Tarifas");
                    }
                    for (int i = 0; i < listaVerificacion.Count; i++)
                    {
                        Funcion funcion = new Funcion();
                        funcion.codEvento = idEvento;
                        funcion.fecha = listaVerificacion[i].fechaFuncion;
                        funcion.horaIni = listaVerificacion[i].horaInicio;
                        funcion.estado = "ACTIVO";
                        db.Funcion.Add(funcion);
                        db.SaveChanges();
                    }
                    ObtenerFechaFin(idEvento);
                    return RedirectToAction("Tarifas");
                }
                ViewBag.MensajeError = "Funciones Repetidas en el mismo dia";
                ViewBag.Resultados = listaVerificacion;
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Tarifas()
        {
            int idEvento = 0;
            //Si un evento se modifica, las tarifas se crean desde 0
            if (Session["IdEventoModificado"] != null || Session["IdEventoCreado"] != null)
            {
                List<PeriodoVenta> listaPV = new List<PeriodoVenta>();
                List<string> nombresPV = new List<string>();
                if (Session["IdEventoCreado"] != null)
                {
                    idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                    listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                    foreach (PeriodoVenta p in listaPV)
                    {
                        nombresPV.Add("Del " + String.Format("{0:dd/MM/yyyy}", p.fechaInicio) + " hasta: " + String.Format("{0:dd/MM/yyyy}", p.fechaFin));
                    }
                    ViewBag.NombrePV = nombresPV;
                    ViewBag.MensajeError = "";
                    return View();
                }
                if (Session["IdEventoModificado"] != null)
                    idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                //NOMBRE DEL evento
                foreach (PeriodoVenta p in listaPV)
                {
                    nombresPV.Add("Del " + String.Format("{0:dd/MM/yyyy}", p.fechaInicio) + " hasta: " + String.Format("{0:dd/MM/yyyy}", p.fechaFin));
                }
                //--//
                List<ZonaEvento> zonasEvento = db.ZonaEvento.Where(c => c.codEvento == idEvento).ToList();
                ZonaEventoListModel model = new ZonaEventoListModel();
                model.ListaZEM = new List<ZonaEventoModel>();
                foreach (ZonaEvento zona in zonasEvento)
                {
                    ZonaEventoModel zonamodel = new ZonaEventoModel();
                    zonamodel.Aforo = zona.aforo;
                    zonamodel.Nombre = zona.nombre;
                    zonamodel.ListaTarifas = new List<TarifaModel>();
                    zonamodel.Id = zona.codZona;
                    List<PrecioEvento> preciosevento = db.PrecioEvento.Where(c => c.codZonaEvento == zona.codZona).ToList();
                    foreach (PrecioEvento precio in preciosevento)
                    {
                        TarifaModel tarifa = new TarifaModel();
                        tarifa.Precio = (Double)precio.precio;
                        zonamodel.ListaTarifas.Add(tarifa);
                    }
                    if (preciosevento.Count < listaPV.Count)
                    {
                        for (int i = 0; i < (listaPV.Count - preciosevento.Count); i++)
                        {
                            TarifaModel tarifa = new TarifaModel();
                            tarifa.Precio = 0;
                            zonamodel.ListaTarifas.Add(tarifa);
                        }
                    }
                    model.ListaZEM.Add(zonamodel);
                }
                ViewBag.TarifasDelEvento = model;
                ViewBag.NombrePV = nombresPV;
                ViewBag.MensajeError = "";
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }

        private void FiltrarTarifas(List<ZonaEventoModel> listaModel, List<PeriodoVenta> listaPV, int idEvento)
        {
            //boro los precio evento existentes par aluego reemplazarlos
            foreach (PeriodoVenta pventa in listaPV)
            {
                List<PrecioEvento> precios = db.PrecioEvento.Where(c => c.codPeriodoVenta == pventa.idPerVent).ToList();
                foreach (PrecioEvento precio in precios)
                {
                    db.PrecioEvento.Remove(precio);
                    db.SaveChanges();
                }
            }

            List<ZonaEvento> zonas = db.ZonaEvento.Where(c => c.codEvento == idEvento).ToList();
            for (int i = 0; i < zonas.Count; i++)
            {
                //si la zona no existe en el model. tengo que eliminarla
                if (!listaModel.Any(c => c.Id == zonas[i].codZona))
                {
                    db.ZonaEvento.Remove(zonas[i]);
                    db.SaveChanges();
                }
            }
            for (int i = 0; i < listaModel.Count; i++)
            {
                int idZona = listaModel[i].Id;
                //es una zona modificada
                if (idZona != 0)
                {
                    ZonaEvento zonaMod = db.ZonaEvento.Where(c => c.codZona == idZona).First();
                    zonaMod.nombre = listaModel[i].Nombre;
                    zonaMod.aforo = listaModel[i].Aforo;
                    db.SaveChanges();
                }
                else
                {
                    //es una zona nueva
                    ZonaEvento zonaNueva = new ZonaEvento();
                    zonaNueva.nombre = listaModel[i].Nombre;
                    zonaNueva.aforo = listaModel[i].Aforo;
                    zonaNueva.codEvento = idEvento;
                    zonaNueva.cantFilas = 0;
                    zonaNueva.cantColumnas = 0;
                    zonaNueva.tieneAsientos = false;
                    db.ZonaEvento.Add(zonaNueva);
                    db.SaveChanges();
                    idZona = zonaNueva.codZona;
                }
                //guardo sus tarifas
                List<TarifaModel> tarifas = listaModel[i].ListaTarifas;
                for (int j = 0; j < tarifas.Count; j++)
                {
                    PrecioEvento precioevento = new PrecioEvento();
                    precioevento.precio = tarifas[j].Precio;
                    precioevento.codPeriodoVenta = listaPV[j].idPerVent;
                    precioevento.codZonaEvento = idZona;
                    db.PrecioEvento.Add(precioevento);
                    db.SaveChanges();
                }
            }
        }

        private void CreaZonasxFuncion(int idEvento)
        {
            List<ZonaEvento> zonas = db.ZonaEvento.Where(c => c.codEvento == idEvento).ToList();
            List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == idEvento).ToList();
            foreach (ZonaEvento zona in zonas)
            {
                foreach (Funcion funcion in funciones)
                {
                    ZonaxFuncion zonaxfuncion = new ZonaxFuncion();
                    zonaxfuncion.cantLibres = zona.aforo;
                    zonaxfuncion.codFuncion = funcion.codFuncion;
                    zonaxfuncion.codZona = zona.codZona;
                    db.ZonaxFuncion.Add(zonaxfuncion);
                    db.SaveChanges();
                }
            }
        }

        private void CreaZonasxFuncion2(int idEvento)
        {
            List<ZonaEvento> zonas = db.ZonaEvento.Where(c => c.codEvento == idEvento).ToList();
            List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == idEvento).ToList();
            foreach (ZonaEvento zona in zonas)
            {
                foreach (Funcion funcion in funciones)
                {
                    if (!db.ZonaxFuncion.Any(c => c.codZona == zona.codZona && c.codFuncion == funcion.codFuncion))
                    {
                        ZonaxFuncion zonaxfuncion = new ZonaxFuncion();
                        zonaxfuncion.cantLibres = zona.aforo;
                        zonaxfuncion.codFuncion = funcion.codFuncion;
                        zonaxfuncion.codZona = zona.codZona;
                        db.ZonaxFuncion.Add(zonaxfuncion);
                        db.SaveChanges();
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Tarifas(ZonaEventoListModel model)
        {
            int idEvento = 0;
            if (Session["IdEventoModificado"] != null || Session["IdEventoCreado"] != null)
            {
                List<PeriodoVenta> listaPV = new List<PeriodoVenta>();
                List<ZonaEventoModel> list = model.ListaZEM;
                if (Session["IdEventoCreado"] != null)
                    idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                if (Session["IdEventoModificado"] != null)
                {
                    idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                    listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                    FiltrarTarifas(list, listaPV, idEvento);
                    CreaZonasxFuncion2(idEvento);
                    return RedirectToAction("ExtrasEvento");
                }
                listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                foreach (ZonaEventoModel zona in list)
                {
                    //guardamos la zona del evento
                    ZonaEvento zonaEvento = new ZonaEvento();
                    zonaEvento.aforo = zona.Aforo;
                    zonaEvento.nombre = zona.Nombre;
                    zonaEvento.codEvento = idEvento;
                    zonaEvento.cantFilas = 0;
                    zonaEvento.cantColumnas = 0;
                    zonaEvento.tieneAsientos = false;
                    db.ZonaEvento.Add(zonaEvento);
                    db.SaveChanges();
                    int idZona = zonaEvento.codZona;
                    List<TarifaModel> funciones = zona.ListaTarifas;
                    for (int i = 0; i < funciones.Count; i++)
                    {
                        PrecioEvento precioEvento = new PrecioEvento();
                        precioEvento.codPeriodoVenta = listaPV[i].idPerVent;
                        precioEvento.codZonaEvento = idZona;
                        precioEvento.precio = funciones[i].Precio;
                        db.PrecioEvento.Add(precioEvento);
                        db.SaveChanges();
                    }
                }
                CreaZonasxFuncion(idEvento);
                return RedirectToAction("ExtrasEvento");
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }

        public ActionResult ExtrasEvento()
        {
            ExtrasModel model = new ExtrasModel();
            if (Session["IdEventoModificado"] != null)
            {
                int idEvento = int.Parse(Session["IdEventoModificado"].ToString());
                Eventos evento = db.Eventos.Find(idEvento);
                model.esDestacado = (evento.ImagenDestacado != null) ? true : false;
                model.tieneSitios = (evento.ImagenSitios != null) ? true : false;
                model.IDestacado = evento.ImagenDestacado;
                model.IEvento = evento.ImagenEvento;
                model.ISitios = evento.ImagenSitios;
                model.Ganancia = (double)(evento.porccomision == null ? 0 : evento.porccomision);
                model.MaxReservas = evento.maxReservas;
                model.MontFijoVentEnt = (double)(evento.montoFijoVentaEntrada == null ? 0 : evento.montoFijoVentaEntrada);
                model.PenCancelacion = (double)(evento.penalidadXcancelacion == null ? 0 : evento.penalidadXcancelacion);
                model.PenPostergacion = (double)(evento.penalidadXpostergacion == null ? 0 : evento.penalidadXpostergacion);
                model.PermitirBoletoElectronico = (bool)evento.tieneBoletoElectronico;
                model.PermitirReservasWeb = (bool)evento.permiteReserva;
                model.PermiteDevolucionPostergacion = evento.devolverPostergacion;
                model.PuntosToCliente = evento.puntosAlCliente;
                return View(model);
            }
            if (Session["IdEventoCreado"] != null)
            {
                model.MaxReservas = db.Politicas.Find(2).valor.Value;
                return View(model);
            }

            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }

        Boolean guardarImagen(string path, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return false;
            var termina = file.FileName.Split('.')[1];

            path = Server.MapPath("/Images") + "/" + path;
            if ((System.IO.File.Exists(path)))
            {
                System.IO.File.Delete(path);
            }
            file.SaveAs(path);

            return true;
        }

        [HttpPost]
        public ActionResult ExtrasEvento(ExtrasModel model)
        {
            int idEvento = 0;
            if (Session["IdEventoCreado"] != null)
                idEvento = int.Parse(Session["IdEventoCreado"].ToString());
            if (Session["IdEventoModificado"] != null)
                idEvento = int.Parse(Session["IdEventoModificado"].ToString());
            Eventos evento = db.Eventos.Find(idEvento);

            if (evento.ImagenEvento == null && (model.ImageEvento == null || model.ImageEvento.ContentLength == 0))
            {
                ModelState.AddModelError("ImageEvento", "Falta Seleccionar Imagen para Evento");
            }

            if (evento.ImagenSitios == null && model.tieneSitios && (model.ImageSitios == null || model.ImageSitios.ContentLength == 0))
            {
                ModelState.AddModelError("ImageSitios", "Falta Seleccionar Imagen para los Sitios");
            }

            if (evento.ImagenDestacado == null && model.esDestacado && (model.ImageDestacado == null || model.ImageDestacado.ContentLength == 0))
            {
                ModelState.AddModelError("ImageDestacado", "Falta Seleccionar Imagen para Evento Destacado");
            }

            if (ModelState.IsValid)
            {
                evento.porccomision = model.Ganancia;

                if (model.esDestacado)
                {
                    /*--No se carga de vuelta la imagen subida-- valida con string pero no con imagenes
                     * al ser un HttpPostFileBase no se puede recuperar porque es una clase abstracta.
                     * si no es destacado simplemente dejar la imagen en null. si agrega otra imagen recien se guarda, si no hay nada simplemente dejarla como estaba antes.
                     * Hay una situacion con poner evento.ImagenDestacado en null, al realizar db.SaveChanges(), me dice que el campo debe ser obligatorio a pesar de que no se especifica en ningun lado de que lo sea. Incluso en base de datos esta permitido el valor de null.*/
                    if (Session["IdEventoCreado"] != null || Session["IdEventoModificado"] != null)
                    {
                        if (guardarImagen("destacados/destacado" + evento.codigo + ".jpg", model.ImageDestacado))
                            evento.ImagenDestacado = "/Images/destacados/" + "destacado" + evento.codigo + ".jpg";
                        else evento.ImagenDestacado = null;
                    }
                }
                else evento.ImagenDestacado = null;

                if (model.tieneSitios)
                {
                    if (Session["IdEventoCreado"] != null || Session["IdEventoModificado"] != null)
                    {
                        if (guardarImagen("asientos/sitios" + evento.codigo + ".jpg", model.ImageSitios))
                            evento.ImagenSitios = "/Images/asientos/" + "sitios" + evento.codigo + ".jpg";
                        else evento.ImagenSitios = null;
                    }
                }
                else evento.ImagenSitios = null;

                if (guardarImagen("eventos/evento" + evento.codigo + ".jpg", model.ImageEvento)) evento.ImagenEvento = "/Images/eventos/" + "evento" + evento.codigo + ".jpg";

                evento.maxReservas = model.MaxReservas;
                evento.montoFijoVentaEntrada = model.MontFijoVentEnt;
                evento.penalidadXcancelacion = model.PenCancelacion;
                evento.penalidadXpostergacion = model.PenPostergacion;
                evento.tieneBoletoElectronico = model.PermitirBoletoElectronico;
                evento.permiteReserva = model.PermitirReservasWeb;
                evento.puntosAlCliente = model.PuntosToCliente;
                evento.devolverPostergacion = model.PermiteDevolucionPostergacion;
                evento.estado = "Activo";
                db.SaveChanges();

                TempData["tipo"] = "alert alert-success";
                if (Session["IdEventoCreado"] != null)
                    TempData["message"] = "Evento Creado Exitosamente.";
                if (Session["IdEventoModificado"] != null)
                    TempData["message"] = "Evento Modificado Exitosamente.";

                Session["IdEventoModificado"] = null;
                Session["IdEventoCreado"] = null;
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Asientos(string evento)
        {
            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = evento;
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();
            ViewBag.yaVencio = (queryEvento.fecha_inicio < DateTime.Today);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            dynamic myObject = new List<dynamic>();

            foreach (ZonaEvento zona in ViewBag.listaZonas)
            {
                List<Asientos> listaAsientos = db.Asientos.Where(xx => xx.codZona == zona.codZona).ToList();

                var posF = new int[listaAsientos.Count];
                var posC = new int[listaAsientos.Count];
                int cnt = 0;
                foreach (var asiento in listaAsientos)
                {
                    posF[cnt] = (int)asiento.fila;
                    posC[cnt] = (int)asiento.columna;
                    cnt++;
                }

                var actZona = new
                {
                    filas = (int)zona.cantFilas,
                    columnas = (int)zona.cantColumnas,
                    posFila = posF,
                    posColumna = posC,
                    tieneAsientos = zona.tieneAsientos,
                    index = zona.codZona,
                };
                myObject.Add(actZona);
            }
            ViewBag.myObject = serializer.Serialize(myObject);
            return View();
        }

        //Borrar Asientos
        [HttpPost]
        public ActionResult Asientos(int idZona)
        {
            ZonaEvento queryZona = db.ZonaEvento.Where(c => c.codZona == idZona).First();
            db.Entry(queryZona).State = EntityState.Modified;
            queryZona.tieneAsientos = false;
            db.SaveChanges();
            db.Asientos.RemoveRange(db.Asientos.Where(x => (x.codZona == idZona)));
            db.SaveChanges();

            int id = queryZona.codEvento;

            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.yaVencio = (queryEvento.fecha_inicio < DateTime.Today);
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = id + "";
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult GenerarAsientos(ZonaModel zona)
        {
            ZonaEvento zonaE = db.ZonaEvento.Where(c => c.codZona == zona.idZona).First();

            //ACA BORRA LOS ASIENTOS
            Asientos(zona.idZona);
            db.Entry(zonaE).State = EntityState.Modified;
            zonaE.cantFilas = zona.filas;
            zonaE.cantColumnas = zona.columnas;
            zonaE.tieneAsientos = true;
            db.SaveChanges();

            int k = zona.posFila.Count;

            // ACA LISTAS LAS FUNCIONES EXISTENTES
            List<Funcion> listFuncion = db.Funcion.Where(x => (x.codEvento == zonaE.codEvento)).ToList();

            for (int i = 0; i < k; i++)
            {
                Asientos asiento = new Asientos();
                asiento.codZona = zona.idZona;
                asiento.fila = zona.posFila[i];
                asiento.columna = zona.posCol[i];

                int idAs;
                using (var context = new inf245netsoft())
                {
                    context.Asientos.Add(asiento); //Inserta Asientos
                    context.SaveChanges();

                    idAs = asiento.codAsiento; // Yes it's here
                }

                // POR CAMBIAR
                //Tengo todas las funciones y hago for
                foreach (var funcion in listFuncion)
                {
                    AsientosXFuncion AXF = new AsientosXFuncion();
                    AXF.codAsiento = idAs;
                    AXF.codFuncion = funcion.codFuncion;
                    AXF.estado = "libre";
                    db.AsientosXFuncion.Add(AXF);
                }
                db.SaveChanges();
            }

            int id = zonaE.codEvento;
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.yaVencio = (queryEvento.fecha_inicio < DateTime.Today);
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = "" + queryEvento.codigo;
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();

            return View("Asientos", new { evento = "" + queryEvento.codigo });
        }

        //TODAVIA NO FUNCIONA
        public object obtenerJSONAsientos(List<Funcion> listFunciones, List<ZonaEvento> listZE)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            List<dynamic> todoObject = new List<dynamic>();

            foreach (Funcion funcion in listFunciones)
            {
                foreach (ZonaEvento zona in listZE)
                {
                    List<int> posF = new List<int>();
                    List<int> posC = new List<int>();
                    var cantLibres = 0;
                    foreach (Asientos asiento in zona.Asientos)
                    {
                        try
                        {
                            AsientosXFuncion asientosFuncion = db.AsientosXFuncion.Find(asiento.codAsiento, funcion.codFuncion);
                            if (asientosFuncion.estado.CompareTo("libre") == 0)
                            {
                                cantLibres++;
                                posF.Add((int)asiento.fila);
                            }
                            else
                            {
                                posF.Add((int)-asiento.fila);

                            }

                            posC.Add((int)asiento.columna);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    if (!zona.tieneAsientos)
                    {
                        ZonaxFuncion zonaFuncion = db.ZonaxFuncion.Find(funcion.codFuncion, zona.codZona);
                        cantLibres = zonaFuncion.cantLibres;
                    }

                    var act = new
                    {
                        filas = (int)zona.cantFilas,
                        columnas = (int)zona.cantColumnas,
                        posFila = posF.ToArray(),
                        posColumna = posC.ToArray(),
                        tieneAsientos = zona.tieneAsientos,
                        totalLibres = cantLibres,
                        indexZE = zona.codZona,
                        indexFH = funcion.codFuncion,
                    };
                    todoObject.Add(act);
                }
            }
            return serializer.Serialize(todoObject);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerEvento(int? id)
        {
            // Si no hay id retorna al Index
            if (id == null) return RedirectToAction("Index");
            var evento = db.Eventos.Find(id);
            // Si no hay evento tambien retorna al Index
            if (evento == null)
            {
                ModelState.AddModelError(string.Empty, "No hay Evento");
                return Redirect("~/Home/Index");
            }
            //Cargamos en el ViewBag el evento
            ViewBag.evento = evento;

            //Cargamos el nombre del local
            try
            {
                ViewBag.NombreLocal = db.Local.Where(c => c.codLocal == evento.idLocal).First().ubicacion;
            }
            catch (Exception ex)
            {
                ViewBag.NombreLocal = "";
                ViewBag.NombreLocal = evento.direccion;
            }

            //Detalle ... Como se considera si ya hay local?
            ViewBag.Region = db.Region.Where(c => c.idRegion == evento.idRegion).First().nombre;
            ViewBag.Categoria = db.Categoria.Where(c => c.idCategoria == evento.idCategoria).First().nombre;
            ViewBag.Subcategoria = db.Categoria.Where(c => c.idCategoria == evento.idSubcategoria).First().nombre;
            var veoAsientos = true;
            //Debo saber si el evento esta a la venta
            if (evento.fecha_fin < DateTime.Today)
            {
                ViewBag.EventoAcabo = "El evento ya no se encuentra disponible.";
                veoAsientos = false;
            }

            int bloqueVenta = 0;
            try
            {
                var buscarPeriodoVenta = db.PeriodoVenta.Where(c => c.codEvento == evento.codigo && c.fechaInicio <= DateTime.Today && c.fechaFin >= DateTime.Today);
                bloqueVenta = buscarPeriodoVenta.First().idPerVent;
            }
            catch (Exception ex)
            {
            }

            if (bloqueVenta == 0)
            {
                ViewBag.NoHayVenta = "No es posible comprar Entradas";
                List<PeriodoVenta> periodos = db.PeriodoVenta.Where(c => DateTime.Today <= c.fechaInicio && c.codEvento == id).OrderBy(c => c.fechaInicio).ToList();
                List<string> futuraVenta = new List<string>();
                foreach (PeriodoVenta per in periodos)
                {
                    futuraVenta.Add("- Del " + String.Format("{0:d}", per.fechaInicio) + " al " + String.Format("{0:d}", per.fechaFin) + ".");
                }
                ViewBag.EventoNoDisponible = "Las entradas del evento aun no estan a la venta. Ventas disponibles:";
                ViewBag.FuturasVentas = futuraVenta;
            }

            List<String> promos = new List<string>(0);
            try
            {
                List<Promociones> listPromos = db.Promociones.Where(c => c.codEvento == evento.codigo).ToList();
                foreach (var pr in listPromos)
                {
                    promos.Add(pr.descripcion);
                }
            }
            catch (Exception ex)
            {
            }

            ViewBag.listPromos = promos;

            List<ZonaEvento> zonasEvento = new List<ZonaEvento>();
            ViewBag.ListZonasNombre = new List<string>();
            ViewBag.ListZonasId = new List<int>();
            ViewBag.ListPeriodos = new List<string>();
            List<List<double>> listaPrecios = new List<List<double>>(0);

            try
            {
                var primero = true;
                zonasEvento = evento.ZonaEvento.ToList();
                int jj = 0, ii = 0;
                foreach (ZonaEvento zona in zonasEvento)
                {

                    ViewBag.ListZonasNombre.Add(zona.nombre);
                    ViewBag.ListZonasId.Add(zona.codZona);
                    jj = 0;
                    listaPrecios.Add(new List<double>(0));

                    foreach (var precio in zona.PrecioEvento)
                    {

                        if (primero)
                        {
                            ViewBag.ListPeriodos.Add("Del " + precio.PeriodoVenta.fechaInicio.Value.ToShortDateString() + " Al " + precio.PeriodoVenta.fechaFin.Value.ToShortDateString());
                        }

                        listaPrecios[ii].Add((double)precio.precio);
                        jj++;
                    }
                    primero = false;
                    ii++;
                }
            }
            catch (Exception ex)
            {
                veoAsientos = false;
            }

            ViewBag.listPrecios = listaPrecios;

            List<Funcion> funciones = new List<Funcion>();
            try
            {
                funciones = db.Funcion.Where(c => c.codEvento == evento.codigo && c.estado != "CANCELADO").ToList();
                if (funciones.Count > 1) ViewBag.textoFunciones = "Desde " + funciones[0].fecha.Value.ToShortDateString() + " hasta " + funciones[funciones.Count - 1].fecha.Value.ToShortDateString();
                else ViewBag.textoFunciones = "Única funcion " + funciones[0].fecha.Value.ToShortDateString();
            }
            catch (Exception ex)
            {
                ViewBag.textoFunciones = "No hay funciones";
            }

            funciones = new List<Funcion>();
            try
            {
                var funcAux = db.Funcion.Where(c => c.codEvento == evento.codigo && c.estado != "CANCELADO").ToList();
                foreach (Funcion fun in funcAux)
                {
                    if (fun.fecha >= DateTime.Now) funciones.Add(fun);
                }
                //agrupo las fechas unicas de las funciones y las ordeno ascendentemente
                //funciones = funciones.GroupBy(c => c.fecha).Select(p => p.First()).OrderBy(c => c.fecha).ToList();
                List<SelectListItem> listaNFunciones = new List<SelectListItem>();
                int i = 0;
                foreach (Funcion fun in funciones)
                {
                    listaNFunciones.Add(new SelectListItem { Text = String.Format("{0:t}", fun.fecha), Value = "" + i });
                    i++;
                }
                ViewBag.FechaFunciones = listaNFunciones;

                ViewBag.ListFunciones = funciones;

                ViewBag.ObjectArrayAsientos = obtenerJSONAsientos(funciones, zonasEvento);
            }
            catch (Exception ex)
            {
                ViewBag.ListFunciones = new List<Funcion>(0);
                veoAsientos = false;
            }

            ViewBag.VeoAsientos = veoAsientos;
            //para que se carguen los destacados al lado
            List<Eventos> listaDestacados = new List<Eventos>(0);
            try
            {
                listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null && c.estado != null && c.estado.CompareTo("Activo") == 0)).ToList();
            }
            catch (Exception ex)
            {

            }
            ViewBag.ListaDestacados = listaDestacados;
            return View(new PaqueteEntradas((int)id));
        }

        private bool validarItemCarrito(PaqueteEntradas paquete)
        {
            //si el carrito esta vacio, lo ingreso
            if (Session["Carrito"] == null)
            {
                TempData["tipo"] = "alert alert-success";
                TempData["message"] = "Entradas agregadas al carrito :)";
                return true;
            }
            //si tiene items tengo que buscar que no este
            bool indicador = true;
            Eventos evento = db.Eventos.Find(paquete.idEvento);
            int maxCompra = evento.maxReservas;
            List<PaqueteEntradas> carrito = (List<PaqueteEntradas>)Session["Carrito"];
            //si existe en el carrito
            if (carrito.Any(c => c.idEvento == paquete.idEvento))
            {
                PaqueteEntradas existente = carrito.Where(c => c.idEvento == paquete.idEvento).First();
                if (maxCompra < existente.cantEntradas + paquete.cantEntradas)
                {//si en conjunto supera el maximo de compra, seteo la cantidad de entradas al maximo
                    existente.cantEntradas = maxCompra;
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Superó el limite de compra del evento. Solo se añadieron " + maxCompra + ".";
                }
                else
                {//si no supera, lo arreglo al carrito
                    existente.cantEntradas += paquete.cantEntradas;
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Entradas agregadas al carrito :) Solo puede agregar " + (maxCompra - existente.cantEntradas) + " entradas más.";
                }
                return indicador;
            }
            else
            {//si no existe en el carrito
                if (paquete.cantEntradas > maxCompra)
                {
                    TempData["tipo"] = "alert alert-warning";
                    TempData["message"] = "No se pudo agregar las entradas al carrito, solo puede comprar hasta  " + maxCompra + " entradas";
                    indicador = false;
                }
                else
                {
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Entradas agregadas al carrito :)";
                }
            }
            Session["Carrito"] = carrito;
            return indicador;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Entradas(PaqueteEntradas paquete, string boton)
        {
            if (boton.CompareTo("registro") == 0)
            {
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "Debe de estar registrado para poder reservar entradas a un evento";
                return RedirectToAction("RegisterClient", "Account");
            }

            if (ModelState.IsValid)
            {
                if (boton.CompareTo("reservar") == 0)
                {
                    string mensaje = reservaAsientos(User.Identity.Name, paquete);

                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Se reservaron correctamente las entradas";

                    if (mensaje.CompareTo("Ok") != 0)
                    {
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = mensaje;
                    }

                    return RedirectToAction("MisReservas", "CuentaUsuario");

                }
                else if (boton.CompareTo("carrito") == 0)
                {
                    //si el carrito es null, creo un nuevo carrito
                    if (Session["Carrito"] == null)
                    {
                        List<PaqueteEntradas> carrito = new List<PaqueteEntradas>();
                        carrito.Add(paquete);
                        Session["Carrito"] = carrito;
                    }
                    else
                    {
                        //si el carrito ya existe agrego a mi lista de paquete
                        List<PaqueteEntradas> carrito = (List<PaqueteEntradas>)Session["Carrito"];
                        carrito.Add(paquete);
                        Session["Carrito"] = carrito;
                    }
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Entradas agregadas al carrito :)";
                    return RedirectToAction("MiCarrito", "CuentaUsuario");
                }
                else if (boton.CompareTo("reservarOrganizador") == 0)
                {
                    string mensaje = reservarOrganizador(paquete);
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Se reservaron correctamente las entradas";

                    if (mensaje.CompareTo("Ok") != 0)
                    {
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = mensaje;
                    }
                    return Redirect("~/Evento/VerEvento/" + paquete.idEvento);
                }
                else if (boton.CompareTo("carritoVendedor") == 0)
                {
                    if (Session["CarritoVendedor"] == null)
                    {
                        List<PaqueteEntradas> carritoV = new List<PaqueteEntradas>();
                        carritoV.Add(paquete);
                        Session["CarritoVendedor"] = carritoV;
                    }
                    else
                    {
                        List<PaqueteEntradas> carritoV = (List<PaqueteEntradas>)Session["CarritoVendedor"];
                        carritoV.Add(paquete);
                        Session["CarritoVendedor"] = carritoV;
                    }
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Item añadido al carrito de ventas";
                    return RedirectToAction("CarritoVentas", "Ventas");
                }
            }
            else
            {
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "Elija asientos!!!";
            }
            return Redirect("~/Evento/VerEvento/" + paquete.idEvento);
        }

        [AllowAnonymous]
        public ActionResult BusquedaPaging(int? page)
        {
            return View();
        }

        [AllowAnonymous]
        // [RequireRequestValue(new[] { "fech_ini", "fech_fin", "idCategoria", "idSubCat", "idRegion", "idProv" })]
        //  [RequireRequestValue(new[] { "nombre"})]
        public ActionResult Busqueda(DateTime? fech_ini, DateTime? fech_fin, int? idCategoria, int? idSubCat, int? idRegion, int? idProv, string nombre, int? page)
        {
            ViewBag.nombre = nombre;
            ViewBag.fech_ini = fech_ini;
            ViewBag.fech_fin = fech_fin;
            ViewBag.idCategoria = idCategoria;
            ViewBag.idSubCat = idSubCat;
            ViewBag.idRegion = idRegion;
            ViewBag.idProv = idProv;
            var lista = from obj in db.Eventos
                        where (obj.estado.Contains("Activo") == true)
                        select obj;
            /*
           var arreglo = from obj in db.Eventos
                         select obj;
         */
            /*
           List<Eventos> lista = arreglo.ToList();
           lista = lista.Where(c => c.estado.Equals("Activo") == true);
           */
            if (fech_ini > fech_fin)
            {
                lista = null;
                ViewBag.Lista = lista;
                var categorias2 = db.Categoria.AsNoTracking().Where(c => c.nivel == 1);
                ViewBag.categorias = new SelectList(categorias2, "idCategoria", "nombre");
                var departamentos2 = db.Region.AsNoTracking().Where(c => c.idRegPadre == null);
                ViewBag.departamentos = new SelectList(departamentos2, "idRegion", "nombre");
                List<Region> listProv2 = new List<Region>();
                List<Categoria> listSubCat2 = new List<Categoria>();
                ViewBag.distritos = new SelectList(listProv2, "idProv", "nombre");
                ViewBag.subcategorias = new SelectList(listSubCat2, "idSubcat", "nombre");

                int pageNumber2 = (page ?? 1);
                int pageSize2 = 8;
                return View(lista.ToPagedList(pageNumber2, pageSize2));
            }
            //if (fech_ini < fech_fin)
            //{
            if (!String.IsNullOrEmpty(nombre))
            {
                lista = lista.Where(s => s.nombre.Contains(nombre));
            }

            if (fech_ini.HasValue)
            {
                lista = lista.Where(c => c.fecha_inicio >= fech_ini || (c.fecha_fin >= fech_ini && c.fecha_inicio < fech_ini));
            }

            if (fech_fin.HasValue)
            {
                lista = lista.Where(c => c.fecha_inicio <= fech_fin);
            }

            if (idCategoria.HasValue)
            {

                lista = lista.Where(c => c.idCategoria == idCategoria);
            }

            if (idSubCat.HasValue)
            {
                lista = lista.Where(c => c.idSubcategoria == idSubCat);
            }

            if (idRegion.HasValue)
            {

                lista = lista.Where(c => c.idRegion == idRegion);
            }

            if (idProv.HasValue)
            {
                lista = lista.Where(c => c.idProvincia == idProv);
            }

            lista = lista.OrderBy(s => s.codigo);
            ViewBag.Cant = lista.Count();
            /*
            if (!nombre.Equals(""))
            {
                ViewBag.Lista = db.Eventos.AsNoTracking().Where(c => (c.fecha_inicio >= fech_ini && c.fecha_inicio <= fech_fin &&
                 c.idCategoria == idCategoria && c.idRegion == idRegion && c.idProvincia == idProv && c.estado.Contains("Activo") && c.nombre.Contains(nombre))).ToList();
            }
            else
            {
                ViewBag.Lista = db.Eventos.AsNoTracking().Where(c => (c.fecha_inicio >= fech_ini && c.fecha_inicio <= fech_fin &&
                  c.idCategoria == idCategoria && c.idRegion == idRegion && c.idProvincia == idProv && c.estado.Contains("Activo"))).ToList();
            }
            */
            var categorias = db.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.categorias = new SelectList(categorias, "idCategoria", "nombre");
            var departamentos = db.Region.AsNoTracking().Where(c => c.idRegPadre == null);
            ViewBag.departamentos = new SelectList(departamentos, "idRegion", "nombre");
            List<Region> listProv = new List<Region>();
            List<Categoria> listSubCat = new List<Categoria>();
            ViewBag.distritos = new SelectList(listProv, "idProv", "nombre");
            ViewBag.subcategorias = new SelectList(listSubCat, "idSubcat", "nombre");

            int pageNumber = (page ?? 1);
            int pageSize = 12;
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        [AllowAnonymous]
        public ActionResult Subcategorias()
        {
            return View();

        }

        [AllowAnonymous]
        public ActionResult Distritos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EnviarComentario(int codEvento, string usuario, string contenido)
        {
            Comentarios new_coment = new Comentarios();

            new_coment.codEvento = codEvento;
            new_coment.contenido = contenido;
            new_coment.usuario = usuario;

            new_coment.fecha = DateTime.Now;
            db.Comentarios.Add(new_coment);
            db.SaveChanges();

            var lista = from comentario in db.Comentarios
                        where comentario.codEvento == codEvento
                        orderby comentario.fecha descending
                        select new Coment
                        {
                            flag = (comentario.usuario == usuario) ? true : false,
                            contenido = comentario.contenido,
                            nombre = comentario.CuentaUsuario.nombre,
                            fecha = comentario.fecha,
                            codigo = comentario.codComentario
                        };

            lista = lista.Take(6);
            List<Coment> listaNueva = lista.ToList<Coment>();
            return Json(listaNueva, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComents(int codEvento, string usuario, string contenido, int? offset)
        {
            Comentarios new_coment = new Comentarios();
            // Comentarios list = db.Comentarios.Last();

            int skipVal = offset ?? 0;
            var lista = from comentario in db.Comentarios
                        where comentario.codEvento == codEvento
                        orderby comentario.codComentario descending
                        select new Coment
                        {
                            flag = (comentario.usuario == usuario) ? true : false,
                            contenido = comentario.contenido,
                            nombre = comentario.CuentaUsuario.nombre,
                            fecha = comentario.fecha,
                            codigo = comentario.codComentario
                        };
            lista = lista.Skip(skipVal).Take(cantMax);
            //  lista = lista.Take(6);
            List<Coment> listaNueva = lista.ToList<Coment>();
            return Json(listaNueva, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DelComment(int cod, int codEvento)
        {

            db.Configuration.AutoDetectChangesEnabled = false;

            Comentarios data = new Comentarios();
            string act = "";
            if (Request.IsAuthenticated)
            {
                act = User.Identity.Name;
            }

            data.codComentario = cod;
            data.codEvento = codEvento;
            data.usuario = act;
            try
            {
                db.Comentarios.Attach(data);
                db.Comentarios.Remove(data);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                db.Configuration.AutoDetectChangesEnabled = true;
            }
            return Json("Eliminado exitoso", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Destacados()
        {
            //para que se carguen los destacados al lado
            List<Eventos> listaDestacados = new List<Eventos>(0);
            try
            {
                listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null && c.estado != null && c.estado.CompareTo("Activo") == 0)).ToList();
            }
            catch (Exception ex)
            {
            }
            ViewBag.ListaDestacados = listaDestacados;
            return View();
        }
    }
}