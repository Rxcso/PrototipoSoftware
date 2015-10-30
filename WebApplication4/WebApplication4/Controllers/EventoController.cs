using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using PagedList;
using System.Data.Entity;
using System.Web.Script.Serialization;
namespace WebApplication4.Controllers
{
    [Authorize]
    public class EventoController : Controller
    {
        inf245netsoft db = new inf245netsoft();
        const int maximoPaginas = 2;
        // GET: Evento
        public ActionResult Index(string nombre, string orden, DateTime? fech_ini, DateTime? fech_fin, int? idCategoria, int? idSubCat, int? idRegion, int? page)
        {
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
                    lista = lista.OrderBy(s => s.codigo);
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
            int pageSize = 3;
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult ModificarEvento(int evento)
        {
            Eventos EventoUp = db.Eventos.Find(evento);
            //verifico que no haya empezado la venta aun
            if (EventoUp.fecha_inicio >= DateTime.Today)
            {
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "El evento ya empezó. No se puede modificar. En caso desee modificarlo consulte con un administrador.";
                return RedirectToAction("Index");
            }
            //Carga de datos
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias && c.activo == 1).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = new List<Categoria>();
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
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idSubCat", "nombre");
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre", model.idRegion);
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre", model.idCategoria);
            ViewBag.NombreOrganizador = db.Organizador.Find(model.idOrganizador).nombOrg;
            ViewBag.NombreLocal = "";
            Local p = new Local();
            try
            {
                p = db.Local.Find(model.Local);
            }
            catch (Exception ex)
            {
                ViewBag.NombreLocal = p.ubicacion;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ModificarEvento(DatosGeneralesModel model)
        {
            int idEvento = 0;
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
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idSubCat", "nombre");
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.MensajeExtra = "Nuevo Evento: Datos Generales";
            return View(model);
        }

        [HttpPost]
        public ActionResult DatosGenerales(DatosGeneralesModel model)
        {
            Session["modelEvento"] = model;
            if (Validaciones.VerificaEventoDG(model))
            {
                if (ModelState.IsValid)
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
                    evento.ImagenDestacado = MagicHelpers.NuevoEvento;
                    db.Eventos.Add(evento);
                    db.SaveChanges();
                    int id = evento.codigo;
                    Session["IdEventoCreado"] = id;
                    return RedirectToAction("BloquesTiempoVenta");

                }
            }
            ViewBag.MensajeExtra = "Valores de organizador y local guardados, puede cambiarlos si desea.";
            ModelState.AddModelError("idLocal", "Se necesita un local o una direccion para el evento");
            ModelState.AddModelError("Direccion", "Se necesita un local o una direccion para el evento");
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias && c.activo == 1).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = new List<Categoria>();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idSubCat", "nombre");
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
                PeriodoVenta pventa = periodo.Where(c => c.fechaInicio == lista[i].fechaInicio && c.fechaFin == lista[i].fechaFin).First();
                //si no exite lo tengo que agregar
                if (pventa == null)
                    agregar.Add(pventa);
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
                List<PrecioEvento> tarifas = db.PrecioEvento.Where(c => c.codPeriodoVenta == periodo[i].idPerVent).ToList();
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
        private void ObtenerFechaInicioyFin(List<BloqueDeTiempoModel> bloques, int idEvento)
        {
            List<DateTime> inicio = new List<DateTime>();
            List<DateTime> fin = new List<DateTime>();
            foreach (BloqueDeTiempoModel bloque in bloques)
            {
                inicio.Add(bloque.fechaInicio);
                fin.Add(bloque.fechaFin);
            }
            inicio.Sort((a, b) => a.CompareTo(b));
            fin.Select((a, b) => b.CompareTo(a));
            DateTime fechaInicio = inicio.First();
            DateTime fechaFin = inicio.First();
            Eventos evento = db.Eventos.Find(idEvento);
            evento.fecha_inicio = fechaInicio;
            evento.fecha_fin = fechaFin;
            db.SaveChanges();
        }
        [HttpPost]
        public ActionResult BloquesTiempoVenta(BloqueTiempoListModel model)
        {
            int idEvento;
            List<BloqueDeTiempoModel> listaVerificacion = null;
            if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento) || int.TryParse(Session["IdEventoModificado"].ToString(), out idEvento))
            {
                listaVerificacion = Validaciones.ValidarBloquesDeTiempoDeVenta(model);
                if (model.esCorrecto)
                {
                    ObtenerFechaInicioyFin(listaVerificacion, idEvento);
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
            if (Session["IdEventoModificado"] != null)
            {
                int idEvento = int.Parse(Session["IdEventoModificado"].ToString());
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
            }
            return View();
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
                Funcion funcion = funciones.Where(c => c.fecha == lista[i].fechaFuncion && c.horaIni == lista[i].horaInicio).First();
                //si no exite lo tengo que agregar
                if (funcion == null)
                    agregar.Add(funcion);
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
        [HttpPost]
        public ActionResult Funciones(FuncionesListModel model)
        {
            List<FuncionesModel> listaVerificacion = null;
            int idEvento;
            if (Session["IdEventoCreado"] != null && Session["IdEventoModificado"] != null)
            {
                if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento) || int.TryParse(Session["IdEventoModificado"].ToString(), out idEvento))
                {
                    listaVerificacion = Validaciones.ValidarFunciones(model);
                    if (model.esCorrecto)
                    {
                        //si solo tiene una funcion, es un evento unico
                        Eventos evento = db.Eventos.Find(idEvento);
                        evento.esUnico = model.ListaFunciones.Count == 1;
                        db.SaveChanges();
                        if (Session["IdEventoModificado"] != null)
                        {
                            FiltrarFunciones(listaVerificacion, idEvento);
                            return RedirectToAction("Tarifas");
                        }
                        for (int i = 0; i < listaVerificacion.Count; i++)
                        {
                            Funcion funcion = new Funcion();
                            funcion.codEvento = idEvento;
                            funcion.fecha = listaVerificacion[i].fechaFuncion;
                            funcion.horaIni = listaVerificacion[i].horaInicio;
                            db.Funcion.Add(funcion);
                            db.SaveChanges();
                        }
                        return RedirectToAction("Tarifas");
                    }
                    ViewBag.MensajeError = "Funciones Repetidas en el mismo dia";
                    ViewBag.Resultados = listaVerificacion;
                    return View();
                }
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay evento en proceso de creación o modificación.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Tarifas()
        {
            int idEvento = 20;
            if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento))
            {
                List<PeriodoVenta> listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                List<string> nombresPV = new List<string>();
                foreach (PeriodoVenta p in listaPV)
                {
                    nombresPV.Add("Del " + String.Format("{0:dd/MM/yyyy}", p.fechaInicio) + " hasta: " + String.Format("{0:dd/MM/yyyy}", p.fechaFin));
                }
                ViewBag.NombrePV = nombresPV;
            }
            ViewBag.MensajeError = "";
            return View();
        }

        [HttpPost]
        public ActionResult Tarifas(ZonaEventoListModel model)
        {
            int idEvento = 0;
            if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento))
            {
                List<PeriodoVenta> listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
                List<ZonaEventoModel> list = model.ListaZEM;
                foreach (ZonaEventoModel zona in list)
                {
                    //guardamos la zona del evento
                    ZonaEvento zonaEvento = new ZonaEvento();
                    zonaEvento.aforo = zona.Aforo;
                    zonaEvento.nombre = zona.Nombre;
                    zonaEvento.codEvento = idEvento;
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
            }
            return RedirectToAction("ExtrasEvento");
        }

        public ActionResult ExtrasEvento()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExtrasEvento(ExtrasModel model)
        {
            if (ModelState.IsValid)
            {
                int idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                var evento = db.Eventos.Find(idEvento);
                evento.porccomision = model.Ganancia;
                evento.ImagenDestacado = "/Images/destacado.png";
                evento.ImagenEvento = "/Images/destacado.png";
                evento.ImagenSitios = "/Images/destacado.png";
                evento.maxReservas = model.MaxReservas;
                evento.montoFijoVentaEntrada = model.MontFijoVentEnt;
                evento.penalidadXcancelacion = model.PenCancelacion;
                evento.penalidadXpostergacion = model.PenPostergacion;
                evento.tieneBoletoElectronico = model.PermitirBoletoElectronico;
                evento.permiteReserva = model.PermitirReservasWeb;
                evento.puntosAlCliente = model.PuntosToCliente;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        public ActionResult Asientos(string evento)
        {
            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = evento;
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();
            ViewBag.estado = (queryEvento.fecha_fin > DateTime.Today);

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
            ViewBag.estado = (queryEvento.fecha_fin > DateTime.Today);
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = id + "";
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult GenerarAsientos(ZonaModel zona)
        {

            ZonaEvento zonaE = db.ZonaEvento.Where(c => c.codZona == zona.idZona).First();

            Asientos(zona.idZona);

            db.Entry(zonaE).State = EntityState.Modified;
            zonaE.cantFilas = zona.filas;
            zonaE.cantColumnas = zona.columnas;
            zonaE.tieneAsientos = true;
            db.SaveChanges();

            int k = zona.posFila.Count;

            Asientos asiento = new Asientos();
            asiento.codZona = zona.idZona;
            for (int i = 0; i < k; i++)
            {
                asiento.fila = zona.posFila[i];
                asiento.columna = zona.posCol[i];
                db.Asientos.Add(asiento); //Insertar asientos
                db.SaveChanges();
            }


            int id = zonaE.codEvento;
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.estado = (queryEvento.fecha_fin > DateTime.Today);
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = "" + queryEvento.codigo;
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();

            return View("Asientos", new { evento = "" + queryEvento.codigo });
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerEvento(int id)
        {
            var evento = db.Eventos.Find(id);
            if (evento == null)
            {
                ModelState.AddModelError(string.Empty, "No hay Evento");
                return Redirect("~/Home/Index");

            }

            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Register(EventoModel model)
        {

            //System.Console.WriteLine("gg");
            if (model.ImageEvento == null || model.ImageEvento.ContentLength == 0)
            {
                ModelState.AddModelError("ImageEvento", "Se necesita la Imagen del Evento");
            }

            if (ModelState.IsValid)
            {
                var eventp = new Eventos();
                if (model.ImageDestacado != null && model.ImageDestacado.ContentLength > 0)
                {
                    var uploadDir = "/Images/";
                    eventp.ImagenDestacado = uploadDir + "destacado" + model.ImageDestacado.FileName;
                    model.ImageDestacado.SaveAs(Server.MapPath("~/Images/" + "destacado" + model.ImageDestacado.FileName));
                }

                eventp.nombre = model.nombre;
                eventp.idOrganizador = 1;
                eventp.idRegion = 1;

                db.Eventos.Add(eventp);
                db.SaveChanges();

                return Redirect("~/Home/Index2");
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult BusquedaPaging(int? page)
        {
            return View();
        }

        [AllowAnonymous]
        // [RequireRequestValue(new[] { "fech_ini", "fech_fin", "idCategoria", "idSubCat", "idRegion", "idProv" })]
        //  [RequireRequestValue(new[] { "nombre"})]
        public ActionResult Busqueda(DateTime? fech_ini, DateTime? fech_fin, int? idCategoria, int? idSubCat, int? idRegion, int? idProv, string nombre = "")
        {

            var lista = from obj in db.Eventos
                        where (obj.estado.Contains("Activo") == true)
                        select obj;

            if (!nombre.Equals(""))
            {
                lista = lista.Where(c => c.nombre.Contains(nombre) == true);
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

            ViewBag.Lista = lista.ToList();

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
            var categorias = db.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.categorias = new SelectList(categorias, "idCategoria", "nombre");
            var departamentos = db.Region.AsNoTracking().Where(c => c.idRegPadre == null);
            ViewBag.departamentos = new SelectList(departamentos, "idRegion", "nombre");
            List<Region> listProv = new List<Region>();
            List<Categoria> listSubCat = new List<Categoria>();

            ViewBag.distritos = new SelectList(listProv, "idProv", "nombre");
            ViewBag.subcategorias = new SelectList(listSubCat, "idSubcat", "nombre");

            return View();
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
        public ActionResult PostergarEvento()
        {
            return View();
        }


    }
}