using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using PagedList;
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
        public ActionResult DatosGenerales()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = new List<Categoria>();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idSubCat", "nombre");
            return View();
        }

        [HttpPost]
        public ActionResult DatosGenerales(DatosGeneralesModel model)
        {
            if (ModelState.IsValid)
            {
                Eventos evento = new Eventos();
                evento.nombre = model.nombre;
                evento.idOrganizador = model.idOrganizador;
                evento.idCategoria = model.idCategoria;
                evento.idSubcategoria = (model.idSubCat == 0) ? 0 : model.idSubCat;
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
                return View("BloquesTiempoVenta");

            }
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == MagicHelpers.Categorias).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = new List<Categoria>();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idSubCat", "nombre");
            return View(model);
        }

        public ActionResult BloquesTiempoVenta()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BloquesTiempoVenta(BloqueTiempoListModel model)
        {
            int idEvento;
            List<BloqueDeTiempoModel> listaVerificacion = null;
            if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento))
            {
                listaVerificacion = Validaciones.ValidarBloquesDeTiempoDeVenta(model);
                if (model.esCorrecto)
                {
                    for (int i = 0; i < listaVerificacion.Count; i++)
                    {
                        PeriodoVenta periodoVenta = new PeriodoVenta();
                        periodoVenta.fechaInicio = listaVerificacion[i].fechaInicio;
                        periodoVenta.fechaFin = listaVerificacion[i].fechaFin;
                        periodoVenta.codEvento = idEvento;
                        db.PeriodoVenta.Add(periodoVenta);
                        db.SaveChanges();
                    }
                    return View("Funciones");
                }
            }
            ViewBag.Resultados = listaVerificacion;
            return View();
        }

        public ActionResult Funciones()
        {
            ViewBag.MensajeError = "";
            return View();
        }

        [HttpPost]
        public ActionResult Funciones(FuncionesListModel model)
        {
            List<FuncionesModel> listaVerificacion = null;
            int idEvento;
            if (Session["IdEventoCreado"] != null)
            {
                if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento))
                {
                    listaVerificacion = Validaciones.ValidarFunciones(model);
                    if (model.esCorrecto)
                    {
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
                }
            }
            ViewBag.MensajeError = "No hay un proceso de registro de evento activo.";
            return View();
        }
        [HttpGet]
        public ActionResult Tarifas()
        {
            int idEvento = 20;
            //if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento))
            //{
            List<PeriodoVenta> listaPV = db.PeriodoVenta.Where(c => c.codEvento == idEvento).ToList();
            List<string> nombresPV = new List<string>();
            foreach (PeriodoVenta p in listaPV)
            {
                nombresPV.Add("Del " + String.Format("{0:dd/MM/yyyy}", p.fechaInicio) + " hasta: " + String.Format("{0:dd/MM/yyyy}", p.fechaFin));
            }
            ViewBag.NombrePV = nombresPV;
            //}
            ViewBag.MensajeError = "";
            return View();
        }

        [HttpPost]
        public ActionResult Tarifas(ZonaEventoListModel model)
        {
            int idEvento = 20;
            //if (int.TryParse(Session["IdEventoCreado"].ToString(), out idEvento))
            //{
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
                    }
                }
            //}
            return View("ExtrasEvento");
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
                //int idEvento = int.Parse(Session["IdEventoCreado"].ToString());
                var evento = db.Eventos.Find(20);
                evento.porccomision = model.Ganancia;
                evento.ImagenDestacado = "/Images/destacado.png";
                evento.ImagenEvento = model.ImageEvento.ToString();
                evento.ImagenSitios = model.ImageSitios.ToString();
                evento.maxReservas = model.MaxReservas;
                evento.montoFijoVentaEntrada = model.MontFijoVentEnt;
                evento.penalidadXcancelacion = model.PenCancelacion;
                evento.penalidadXpostergacion = model.PenPostergacion;
                evento.tieneBoletoElectronico = model.PermitirBoletoElectronico;
                evento.permiteReserva = model.PermitirReservasWeb;
                evento.puntosAlCliente = model.PuntosToCliente;
                db.SaveChanges();
                return View("Index", "Evento");
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

            return View();
        }

        //Borrar Asientos
        [HttpPost]
        public ActionResult Asientos(int idZona)
        {

            ZonaEvento queryZona = db.ZonaEvento.Where(c => c.codZona == idZona).First();

            db.Asientos.RemoveRange(db.Asientos.Where(x => (x.codZona == idZona)));
            db.SaveChanges();

            int id = queryZona.codEvento;

            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = id + "";
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult GenerarAsientos(ZonaModel zona)
        {

            ZonaEvento zonaE = db.ZonaEvento.Where(c => c.codZona == zona.idZona).First();

            zonaE.cantFilas = zona.filas;
            zonaE.cantColumnas = zona.columnas;
            zonaE.tieneAsientos = true;

            Asientos(zona.idZona);

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
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = "" + queryEvento.codigo;
            ViewBag.listaZonas = db.ZonaEvento.Where(c => c.codEvento == id).ToList();

            return Redirect("~/Evento/Asientos/?evento=" + id);
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
    }
}