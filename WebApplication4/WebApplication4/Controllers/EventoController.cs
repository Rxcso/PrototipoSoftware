﻿using System;
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
    [Authorize]
    public class EventoController : Controller
    {
        inf245netsoft db = new inf245netsoft();
        const int maximoPaginas = 2;
        const int cantMax = 6;
        // GET: Evento

        public int BuscarEntradasLeQuedan(string name, int idFuncion)
        {
            CuentaUsuario cuenta = db.CuentaUsuario.Find(name);
            int tipoDoc = (int)cuenta.tipoDoc;
            string doc = cuenta.codDoc;

            List<VentasXFuncion> listVXF = db.VentasXFuncion.Where(x => x.codFuncion == idFuncion).ToList();

            Funcion funcion = db.Funcion.Find(idFuncion);
            int limEntradas = (funcion.Eventos.maxReservas == null ? 10000 : (int)funcion.Eventos.maxReservas);
            int actualEntradas = 0;
            foreach (VentasXFuncion VXF in listVXF) if (VXF.Ventas.codDoc.CompareTo(doc) == 0 && VXF.Ventas.tipoDoc == tipoDoc)
                {
                    actualEntradas += VXF.cantEntradas;
                }
            return limEntradas - actualEntradas;
        }
        public int BuscaEntradasQueQuedan(int idFuncion, int idZona)
        {
            ZonaxFuncion zonaxfuncion = db.ZonaxFuncion.Where(c => c.codFuncion == idFuncion && c.codZona == idZona).First();
            return zonaxfuncion.cantLibres;
        }

        public string reservarOrganizador(PaqueteEntradas paquete)
        {
            try
            {
                using (var context = new inf245netsoft())
                {
                    try
                    {  

                        if (paquete.tieneAsientos)
                        {
                            for (int i = 0; i < paquete.cantEntradas; i++)
                            {
                                int col = paquete.columnas[i];
                                int fil = paquete.filas[i];
                                List<Asientos> listasiento = context.Asientos.Where(x => x.codZona == paquete.idZona && x.fila == fil && x.columna == col).ToList();
                                AsientosXFuncion actAsiento = context.AsientosXFuncion.Find(listasiento.First().codAsiento, paquete.idFuncion);
                                actAsiento.estado = "RESERVAORGANIZADOR";
                            }
                        }
                        else
                        {
                            ZonaxFuncion ZXF = context.ZonaxFuncion.Find(paquete.idFuncion, paquete.idZona);
                            if (ZXF.cantLibres < paquete.cantEntradas) return "No hay suficientes entradas";
                            ZXF.cantLibres -= paquete.cantEntradas;
                            ZXF.cantReservaOrganizador += paquete.cantEntradas;
                        }
                        context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        return "No se pudieron reservar los asientos, alguien más ya lo hizo.";
                    }

                }
            }
            catch (Exception ex)
            {
                return "Ocurrio un error inesperado.";
            }
            //Funciones Utilitarias necesarias
            //BuscarEntradasLeQuedan( User , Funcion )
            return "Ok";

        }

        public string reservaAsientos(string name, PaqueteEntradas paquete)
        {
            //name es el correo de la persona
            //PIER ACA VA LA LOGICA DEL GUARDAR
            //LA IDEA ES QUE RETORNE UN STRING CON EL ERROR EN CASO HUBIERA ALGUNO
            //"Superaria el limte de reservas para esta funcion ya tiene n entradas"
            //"Ya no se encuentran disponibles esas entradas"            

            //Si todo esta bien se devuelve OK
            //Que significa todo Ok?
            //Primero busca cuantas entradas mas puede comprar/reservar esta persona para esa funcion
            //Si supera el limite fue ps
            try
            {
                int quedan = BuscarEntradasLeQuedan(name, paquete.idFuncion);

                if (quedan == 0)
                {
                    return "Ya no le quedan reservas/compras disponibles para el evento";
                }

                if (quedan < paquete.cantEntradas)
                {
                    return "No se pudo realizar la reserva, solo puede reservar hasta  " + quedan + "entradas";
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
                        //Ventas vel = db.Ventas.ToList().Last();
                        DateTime hoy = DateTime.Now.Date;
                        ZonaEvento zo = db.ZonaEvento.Find(paquete.idZona);
                        PeriodoVenta per = db.PeriodoVenta.Where(c => c.codEvento == paquete.idEvento && c.fechaInicio <= hoy && c.fechaFin >= hoy).ToList().First();
                        PrecioEvento pr = db.PrecioEvento.Where(c => c.codZonaEvento == paquete.idZona && c.codPeriodoVenta == per.idPerVent).ToList().First();
                        //ve.codVen = vel.codVen + 1;
                        CuentaUsuario cuenta = (CuentaUsuario)Session["UsuarioLogueado"];
                        ve.fecha = DateTime.Now;
                        ve.cantAsientos = paquete.cantEntradas;
                        ve.cliente = cuenta.usuario;
                        ve.codDoc = cuenta.codDoc;
                        ve.Estado = "Reservado";
                        ve.tipoDoc = cuenta.tipoDoc;
                        ve.montoEfectivoSoles = paquete.cantEntradas * pr.precio;
                        ve.MontoTotalSoles = paquete.cantEntradas * pr.precio;
                        db.Ventas.Add(ve);
                        db.SaveChanges();
                        VentasXFuncion vf = new VentasXFuncion();
                        vf.codVen = ve.codVen;
                        vf.cantEntradas = paquete.cantEntradas;
                        vf.codFuncion = paquete.idFuncion;
                        vf.Ventas = ve;
                        vf.Funcion = db.Funcion.Find(paquete.idFuncion);
                        vf.descuento = 0;
                        vf.subtotal = paquete.cantEntradas * pr.precio;
                        vf.total = paquete.cantEntradas * pr.precio;
                        db.VentasXFuncion.Add(vf);
                        //db.SaveChanges();
                        DetalleVenta dt = new DetalleVenta();
                        dt.cantEntradas = paquete.cantEntradas;
                        //dt.codDetalleVenta = db.DetalleVenta.ToList().Last().codDetalleVenta + 1;
                        dt.codFuncion = paquete.idFuncion;
                        dt.codPrecE = pr.codPrecioEvento;
                        dt.total = paquete.cantEntradas * pr.precio;
                        dt.entradasDev = 0;
                        dt.descTot = 0;
                        dt.codVen = vf.codVen;
                        db.DetalleVenta.Add(dt);
                        if (paquete.filas != null && paquete.filas.Count > 0) paquete.tieneAsientos = true;
                        db.SaveChanges();
                        if (paquete.tieneAsientos)
                        {
                            for (int i = 0; i < paquete.cantEntradas; i++)
                            {
                                int col = paquete.columnas[i];
                                int fil = paquete.filas[i];
                                List<Asientos> listasiento = context.Asientos.Where(x => x.codZona == paquete.idZona && x.fila == fil && x.columna == col).ToList();
                                AsientosXFuncion actAsiento = context.AsientosXFuncion.Find(listasiento.First().codAsiento, paquete.idFuncion);
                                actAsiento.estado = "OCUPADO";
                                actAsiento.codDetalleVenta = dt.codDetalleVenta;
                                actAsiento.PrecioPagado = pr.precio;
                            }
                        }
                        else
                        {
                            ZonaxFuncion ZXF = context.ZonaxFuncion.Find(paquete.idFuncion, paquete.idZona);
                            if (ZXF.cantLibres < paquete.cantEntradas) return "No hay suficientes entradas";
                            ZXF.cantLibres -= paquete.cantEntradas;
                        }
                        db.SaveChanges();
                        context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        return "No se pudieron reservar los asientos, alguien más ya lo hizo.";
                    }

                }
            }
            catch (Exception ex)
            {
                return "Ocurrio un error inesperado.";
            }
            //Funciones Utilitarias necesarias
            //BuscarEntradasLeQuedan( User , Funcion )
            return "Ok";
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
                    lista = lista.OrderBy(s => s.fecha_inicio);
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
                if (Validaciones.VerificaEventoDG(model))
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
                        db.SaveChanges();
                        return RedirectToAction("BloquesTiempoVenta");
                    }
                }
                if (model.Local != 0 && model.Direccion != null)
                {
                    ModelState.AddModelError("Local", "Debe ingresar un local o dirección.");
                    ModelState.AddModelError("Direccion", "Debe ingresar una dirección o un local");
                }

                return View(model);
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
            if (Validaciones.VerificaEventoDG(model))
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
                    evento.estado = "Activo";
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
            }
            if (model.idOrganizador == 0)
                ModelState.AddModelError("idOrganizador", "El evento debe tener un organizador");
            if (model.Local != 0 && model.Direccion != null)
            {
                ModelState.AddModelError("Local", "Debe ingresar un local o dirección.");
                ModelState.AddModelError("Direccion", "Debe ingresar una dirección o un local");
            }
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
            List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == idEvento).ToList();
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
                model.IDestacado = evento.ImagenDestacado;
                model.IEvento = evento.ImagenEvento;
                model.ISitios = evento.ImagenSitios;
                model.Ganancia = (double)(evento.porccomision == null ? 0 : evento.porccomision);
                model.MaxReservas = (int)(evento.maxReservas == null ? 0 : evento.maxReservas);
                model.MontFijoVentEnt = (double)(evento.montoFijoVentaEntrada == null ? 0 : evento.montoFijoVentaEntrada);
                model.PenCancelacion = (double)(evento.penalidadXcancelacion == null ? 0 : evento.penalidadXcancelacion);
                model.PenPostergacion = (double)(evento.penalidadXpostergacion == null ? 0 : evento.penalidadXpostergacion);
                model.PermitirBoletoElectronico = (bool)evento.tieneBoletoElectronico;
                model.PermitirReservasWeb = (bool)evento.permiteReserva;
                model.PuntosToCliente = evento.puntosAlCliente;
                return View(model);
            }
            if (Session["IdEventoCreado"] != null)
                return View(model);
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

            if (evento.ImagenSitios == null && (model.ImageSitios == null || model.ImageSitios.ContentLength == 0))
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
                    if (Session["IdEventoCreado"] != null)
                    {
                        if (guardarImagen("destacado" + evento.codigo + ".jpg", model.ImageDestacado))
                            evento.ImagenDestacado = "/Images/" + "destacado" + evento.codigo + ".jpg";
                        else evento.ImagenDestacado = null;
                    }
                }

                if (guardarImagen("evento" + evento.codigo + ".jpg", model.ImageEvento)) evento.ImagenEvento = "/Images/" + "evento" + evento.codigo + ".jpg";
                if (guardarImagen("sitios" + evento.codigo + ".jpg", model.ImageSitios)) evento.ImagenSitios = "/Images/" + "sitios" + evento.codigo + ".jpg";

                evento.maxReservas = model.MaxReservas;
                evento.montoFijoVentaEntrada = model.MontFijoVentEnt;
                evento.penalidadXcancelacion = model.PenCancelacion;
                evento.penalidadXpostergacion = model.PenPostergacion;
                evento.tieneBoletoElectronico = model.PermitirBoletoElectronico;
                evento.permiteReserva = model.PermitirReservasWeb;
                evento.puntosAlCliente = model.PuntosToCliente;
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
            if (id == null) return RedirectToAction("Index");
            var evento = db.Eventos.Find(id);
            if (evento == null)
            {
                ModelState.AddModelError(string.Empty, "No hay Evento");
                return Redirect("~/Home/Index");
            }

            ViewBag.evento = evento;

            try
            {
                ViewBag.NombreLocal = db.Local.Where(c => c.codLocal == evento.idLocal).First().ubicacion;
            }
            catch (Exception ex)
            {
                ViewBag.NombreLocal = "";
            }

            ViewBag.Region = db.Region.Where(c => c.idRegion == evento.idRegion).First().nombre;
            ViewBag.Categoria = db.Categoria.Where(c => c.idCategoria == evento.idCategoria).First().nombre;
            ViewBag.Subcategoria = db.Categoria.Where(c => c.idCategoria == evento.idSubcategoria).First().nombre;

            var veoAsientos = true;

            //Debo saber si el evento esta a la venta
            if (evento.fecha_fin <= DateTime.Today)
            {
                ViewBag.EventoAcabo = "El evento ya no se encuentra disponible.";
                veoAsientos = false;
            }
            else
            {
                try
                {
                    ViewBag.ListZonasNombre = new List<string>();
                    ViewBag.ListZonasId = new List<int>();
                    int bloqueVenta = db.PeriodoVenta.Where(c => c.codEvento == evento.codigo && c.fechaInicio <= DateTime.Today && c.fechaFin >= DateTime.Today).First().idPerVent;
                    List<ZonaEvento> zonasEvento = db.ZonaEvento.Where(c => c.codEvento == id).ToList();
                    List<SelectListItem> tarifasEvento = new List<SelectListItem>();
                    foreach (ZonaEvento zona in zonasEvento)
                    {

                        // Modificacion Oscar
                        if (zona.tieneAsientos == true) veoAsientos = true;

                        //
                        PrecioEvento precio = db.PrecioEvento.Where(c => c.codPeriodoVenta == bloqueVenta && c.codZonaEvento == zona.codZona).ToList().First();
                        ViewBag.ListZonasNombre.Add(zona.nombre + " - " + " S/." + precio.precio);
                        ViewBag.ListZonasId.Add(zona.codZona);
                    }

                    try
                    {
                        List<Funcion> funciones = db.Funcion.Where(c => c.codEvento == evento.codigo && c.estado!="CANCELADO" &&   c.fecha >= DateTime.Now).ToList();

                        //agrupo las fechas unicas de las funciones y las ordeno ascendentemente
                        funciones = funciones.GroupBy(c => c.fecha).Select(p => p.First()).OrderBy(c => c.fecha).ToList();
                        List<SelectListItem> listaNFunciones = new List<SelectListItem>();
                        int i = 0;
                        foreach (Funcion fun in funciones)
                        {
                            listaNFunciones.Add(new SelectListItem { Text = String.Format("{0:t}", fun.fecha), Value = "" + i });
                            i++;
                        }
                        ViewBag.FechaFunciones = listaNFunciones;
                        //todo: buscar tarifa y precio y ver segun que bloque de tiempo estamos

                        //todo: cantidad de entradas depende del numero de asientos que escoja
                        ViewBag.ListFunciones = funciones;
                        ViewBag.ObjectArrayAsientos = obtenerJSONAsientos(funciones, zonasEvento);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.MensajeErrorFunciones = "El evento no cuenta con funciones";
                        veoAsientos = false;
                    }
                }
                catch (Exception ex)
                {
                    List<PeriodoVenta> periodos = db.PeriodoVenta.Where(c => DateTime.Today < c.fechaInicio && c.codEvento == id).OrderBy(c => c.fechaInicio).ToList();
                    List<string> futuraVenta = new List<string>();
                    foreach (PeriodoVenta per in periodos)
                    {
                        futuraVenta.Add("- Del " + String.Format("{0:d}", per.fechaInicio) + " al " + String.Format("{0:d}", per.fechaFin) + ".");
                    }
                    ViewBag.EventoNoDisponible = "Las entradas del evento aun no estan a la venta. Ventas disponibles:";
                    veoAsientos = false;
                    ViewBag.FuturasVentas = futuraVenta;
                }
            }

            ViewBag.VeoAsientos = veoAsientos;
            return View(new PaqueteEntradas((int)id));
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Entradas(PaqueteEntradas paquete, string boton)
        {
            //VALIDAR
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

                    return Redirect("~/Evento/VerEvento/" + paquete.idEvento);
                    //logica de reserva
                    //PLZ

                }
                else if (boton.CompareTo("carrito") == 0)
                {
                    int quedan = BuscaEntradasQueQuedan(paquete.idFuncion,paquete.idZona);

                    if (quedan == 0)
                    {
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = "Ya quedan entradas disponibles para el evento.";
                        return Redirect("~/Evento/VerEvento/" + paquete.idEvento);
                    }
                    if (quedan < paquete.cantEntradas)
                    {
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = "No se pudo agregar entradas al carrito, solo puede comprar " + quedan + " entradas como maximo.";
                        return Redirect("~/Evento/VerEvento/" + paquete.idEvento);
                    }
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
                }
                else if (boton.CompareTo("reservarOrganizador") == 0)
                {
                    string mensaje = reservarOrganizador(  paquete );
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Se reservaron correctamente las entradas";

                    if (mensaje.CompareTo("Ok") != 0)
                    {
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = mensaje;
                    }

                    return Redirect("~/Evento/VerEvento/" + paquete.idEvento);
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
            int pageSize = 8;
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

        /*
         *POSTERGAR EVENTO 
         * 
        */
        [HttpGet]
        public ActionResult PostergarEvento(string evento)
        {
            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.nombreEvento = queryEvento.nombre;
            int idOrganizador = (int)queryEvento.idOrganizador;
            ViewBag.idEvento = evento;
            ViewBag.organizadorEvento = db.Organizador.Where(c => c.codOrg == idOrganizador).First().nombOrg;
            ViewBag.listaFunciones = db.Funcion.Where(c => c.codEvento == id && c.estado != "CANCELADO").ToList();
            //lo bravo
            return View();
        }

        [HttpPost]
        public ActionResult PostergarEvento(PostergarModel evento)
        {

            Funcion funcionAPostergar = db.Funcion.Where(c => (c.codFuncion == evento.idFuncion)).First();
            db.Entry(funcionAPostergar).State = EntityState.Modified;

            funcionAPostergar.fecha = evento.proximaFecha;
            funcionAPostergar.horaIni = evento.proximaHora;
            funcionAPostergar.estado = "POSTERGADO";

            int id = evento.idEvento;
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();

            db.Entry(queryEvento).State = EntityState.Modified;
            queryEvento.hanPostergado = true;
            db.SaveChanges();

            ViewBag.nombreEvento = queryEvento.nombre;
            int idOrganizador = (int)queryEvento.idOrganizador;
            ViewBag.idEvento = "" + id;
            ViewBag.organizadorEvento = db.Organizador.Where(c => c.codOrg == idOrganizador).First().nombOrg;
            ViewBag.listaFunciones = db.Funcion.Where(c => c.codEvento == id && c.estado != "CANCELADO").ToList();

            return View();
        }

        /*
         *CANCELAR EVENTO 
         * 
        */
        [HttpGet]
        public ActionResult CancelarEvento(string evento)
        {

            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();

            List<Funcion> listaFunciones = db.Funcion.Where(c => c.codEvento == id && c.estado != "CANCELADO").ToList();

            CancelarModel cancelarEvento = new CancelarModel();
            cancelarEvento.idEvento = id;
            cancelarEvento.nombreEvento = queryEvento.nombre;
            cancelarEvento.organizador = db.Organizador.Where(c => c.codOrg == queryEvento.idOrganizador).First().nombOrg;

            var auxlistIdFuncion = new List<int>(0);
            var auxlistDateFuncion = new List<DateTime>(0);
            var auxlistBool = new List<Boolean>(0);

            for (int i = 0; i < listaFunciones.Count(); i++)
            {
                auxlistBool.Add(false);
                auxlistDateFuncion.Add((DateTime)listaFunciones[i].horaIni);
                auxlistIdFuncion.Add(listaFunciones[i].codFuncion);
            }

            cancelarEvento.listDateFuncion = auxlistDateFuncion.ToArray();
            cancelarEvento.listIdFuncion = auxlistIdFuncion.ToArray();
            cancelarEvento.seCancela = auxlistBool.ToArray();

            return View("Cancelar", cancelarEvento);
        }

        [HttpPost]
        public ActionResult CancelarEvento(CancelarModel evento)
        {
            if (evento.fechaRecojo <= DateTime.Today)
            {
                ModelState.AddModelError("fechaRecojo", "Elija una fecha posterior al día de hoy");
            }

            int cnt = 0;
            if (evento.seCancela != null)
                for (int i = 0; i < evento.seCancela.Count(); i++)
                    if (evento.seCancela[i]) cnt++;

            if (cnt == 0)
            {
                ModelState.AddModelError("organizador", "Debe elegir un evento a cancelar");
            }

            if (ModelState.IsValid)
            {
                Eventos eventoACancelar = db.Eventos.Find(evento.idEvento);
                db.Entry(eventoACancelar).State = EntityState.Modified;
                eventoACancelar.hanCancelado = true;
                eventoACancelar.estado = "Cancelado";
                db.SaveChanges();

                for (int i = 0; i < evento.seCancela.Count(); i++) if (evento.seCancela[i])
                    {
                        int idF = evento.listIdFuncion[i];
                        Funcion f = db.Funcion.Where(c => c.codFuncion == idF).First();

                        db.Entry(f).State = EntityState.Modified;
                        f.estado = "CANCELADO";
                        f.motivoCambio = evento.motivo;
                        f.FechaDevolucion = evento.fechaRecojo;
                        f.cantDiasDevolucion = evento.diasRecojo;
                        db.SaveChanges();
                    }

                TempData["message"] = "Se cancelar las funciones correctamente";
                TempData["tipo"] = "alert alert-success";
                return Redirect("~/Evento");
            }
            return CancelarEvento("" + evento.idEvento);
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



    }
}