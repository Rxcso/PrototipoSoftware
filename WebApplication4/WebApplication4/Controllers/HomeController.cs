using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using PagedList;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        inf245netsoft db = new inf245netsoft();

        public ActionResult Index(string nombre, DateTime? fech_ini, DateTime? fech_fin, int? idCategoria, int? idSubCat, int? idRegion, int? page)
        {

            List<Eventos> listaDestacados = new List<Eventos>(0);
            try
            {
                listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null && c.estado != null && c.estado.CompareTo("Activo") == 0)).ToList();
            }
            catch (Exception ex)
            {

            }
            ViewBag.ListaDestacados = listaDestacados;

            List<Region> listProv = new List<Region>();
            List<Categoria> listSubCat = new List<Categoria>();

            ViewBag.distritos = new SelectList(listProv, "idProv", "nombre");
            ViewBag.subcategorias = new SelectList(listSubCat, "idSubcat", "nombre");

            int pageNumber = (page ?? 1);
            int pageSize = 12;

            ViewBag.nombre = nombre;
            ViewBag.fech_ini = fech_ini;
            ViewBag.fech_fin = fech_fin;
            ViewBag.idCategoria = idCategoria;
            ViewBag.idSubCat = idSubCat;
            ViewBag.idRegion = idRegion;

            var lista = from obj in db.Eventos 
                        where (obj.estado.Contains("Activo") == true)
                        select obj;

            if (User.Identity.IsAuthenticated && String.IsNullOrEmpty(nombre) && !fech_ini.HasValue && !fech_fin.HasValue && !idCategoria.HasValue && !idSubCat.HasValue && !idRegion.HasValue)
            {
                var auxlista = from cate  in db.Categoria
                               join u in db.CuentaUsuario on User.Identity.Name equals u.correo
                               where ( u.Categoria.Where(s=>s.idCategoria == cate.idCategoria).ToList().Count() > 0 )
                               select cate ;

                auxlista.GroupBy(s => s.idCategoria);

                lista = from obj  in db.Eventos
                        where (obj.estado.Contains("Activo") == true )
                        orderby (
                            (auxlista).Any(s => s.idCategoria == obj.idSubcategoria || s.idCategoria == obj.idCategoria) ? 0 : 1)
                        select obj ;

            }
            else
            {

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

                lista = lista.OrderBy(s => s.fecha_inicio);
            }
            
            ViewBag.Cant = lista.Count();

            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult PuntoVentas()
        {
            ViewBag.Message = "Your application description page.";
            //destacados
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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Search3(string region)
        {
            int id = int.Parse(region);
            List<PuntoVenta> listaPunto;
            if (region == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["ListaP2"] = null;
                return RedirectToAction("PuntoVentas", "Home");
            }
            if (region == "0")
            {
                Session["ListaP2"] = db.PuntoVenta.AsNoTracking().Where(c => c.estaActivo == true).ToList();
                return RedirectToAction("PuntoVentas", "Home");
            }
            listaPunto = db.PuntoVenta.AsNoTracking().Where(c => c.idRegion == id && c.estaActivo == true).ToList();
            if (listaPunto != null) Session["ListaP2"] = listaPunto;
            else Session["ListaP2"] = null;
            return RedirectToAction("PuntoVentas", "Home");
        }

        [HttpPost]
        public ActionResult Subcategorias(int idCategoria)
        {
            var lista = from obj in db.Categoria
                        where (obj.idCatPadre == idCategoria && obj.activo == 1)
                        select new Subcategoria
                        {
                            id = obj.idCategoria,
                            nombre = obj.nombre
                        };
            List<Subcategoria> listaNueva = lista.ToList<Subcategoria>();
            return Json(listaNueva, JsonRequestBehavior.AllowGet);
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