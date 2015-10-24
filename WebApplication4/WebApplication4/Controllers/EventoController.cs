using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using MvcPaging;
namespace WebApplication4.Controllers
{
    public class EventoController : Controller
    {
        inf245netsoft db = new inf245netsoft();
        private const int DefaultPageSize = 2;

        

        
        Evento2Model modelo = new Evento2Model(12);
        // GET: Evento
        public ActionResult Index()
        {
            return View();
          
        }

        [HttpGet]
        public ActionResult Register()
        {
            List<Region> listaDep = db.Region.Where(c => c.idRegPadre == null).ToList();
            List<Region> listProv = new List<Region>();
            ViewBag.DepID = new SelectList(listaDep, "idRegion", "nombre");
            ViewBag.ProvID = new SelectList(listProv, "idProv", "nombre");
            List<Categoria> listaCat = db.Categoria.Where(c => c.idCatPadre == null).ToList();
            listaCat = listaCat.Where(c => c.activo == 1).ToList();
            List<Categoria> listSubCat = new List<Categoria>();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");
            ViewBag.SubID = new SelectList(listSubCat, "idSubCat", "nombre");
            return View();
        }

        
        [HttpGet]
        public ActionResult Asientos(string evento)
        {
            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.nombreEvento = queryEvento.nombre;
            ViewBag.idEvento = evento;
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerEvento(int id)
        {
            var evento = db.Eventos.Find(id);
            if (evento == null)
            {
                ModelState.AddModelError( string.Empty, "No hay Evento" );   
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
            if (model.ImageEvento == null || model.ImageEvento.ContentLength == 0){
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

            
         


            modelo.ListaEventos = db.Eventos.AsNoTracking().Where(c => c.estado.Contains("Activo")).ToList();
            IList<Eventos> eventos = modelo.ListaEventos;



          int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
           
            return View(eventos.ToPagedList(currentPageIndex, DefaultPageSize));


        }
        

        public ActionResult Busqueda(string busqueda = "" )
        {




            if (!busqueda.Equals(""))
            {
                modelo.ListaEventos = db.Eventos.AsNoTracking().Where(c => c.nombre.Contains(busqueda)).ToList();

            }
            else
            {

                modelo.ListaEventos = db.Eventos.AsNoTracking().Where(c => c.estado.Contains("Activo")).ToList();


            }


           

            ViewBag.Lista = modelo.ListaEventos;

            var categorias = db.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.categorias = new SelectList(categorias, "idCategoria", "nombre");
            var departamentos = db.Region.AsNoTracking().Where(c => c.idRegPadre == null);
            ViewBag.departamentos = new SelectList(departamentos, "idRegion", "nombre");
            List<Region> listProv = new List<Region>();
            List<Categoria> listSubCat = new List<Categoria>();



            ViewBag.distritos = new SelectList(listProv, "idProv", "nombre");
            ViewBag.subcategorias = new SelectList(listSubCat, "idSubcat", "nombre");

            return View(modelo);


        }
        [RequireRequestValue(new[] { "fech_ini", "fech_fin", "idCategoria", "idSubCat", "idRegion", "idProv" })]
        public ActionResult Busqueda(DateTime fech_ini, DateTime fech_fin, int idCategoria, int idSubCat, int idRegion, int idProv)
        {
            /* var data = from obj in db.Eventos
                        where (obj.idCategoria == idCategoria && obj.idRegion == idRegion && obj. ) 
  */


            /*
                        var lista = from obj in db.Eventos
                                    where (obj.fecha_inicio >= fech_ini && obj.fecha_inicio <= fech_fin && obj.idCategoria == idCategoria &&
                                    obj.idSubcategoria == idSubCat && obj.idRegion == idRegion && obj.idProvincia == idProv)
                                    select new Eventos;
                                    */
            modelo.ListaEventos = db.Eventos.AsNoTracking().Where(c => (c.fecha_inicio >= fech_ini && c.fecha_inicio <= fech_fin && 
             c.idCategoria == idCategoria && c.idRegion == idRegion && c.idProvincia == idProv && c.estado.Contains("Activo") )).ToList();

            ViewBag.Lista = modelo.ListaEventos;
            var categorias = db.Categoria.AsNoTracking().Where(c => c.nivel == 1);
            ViewBag.categorias = new SelectList(categorias, "idCategoria", "nombre");
            var departamentos = db.Region.AsNoTracking().Where(c => c.idRegPadre == null);
            ViewBag.departamentos = new SelectList(departamentos, "idRegion", "nombre");
           List<Region> listProv = new List<Region>();
          List<Categoria> listSubCat = new List<Categoria>();

           ViewBag.distritos = new SelectList(listProv, "idProv", "nombre");
            ViewBag.subcategorias = new SelectList(listSubCat, "idSubcat", "nombre");

            return View(modelo);


        }

        public ActionResult Subcategorias()
        {

            return View();

        }

        public ActionResult Distritos()
        {

            return View();

        }

        




    }
}