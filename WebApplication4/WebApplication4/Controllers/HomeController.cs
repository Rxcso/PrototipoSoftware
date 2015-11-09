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

            ViewBag.nombre = nombre;
            ViewBag.fech_ini = fech_ini;
            ViewBag.fech_fin = fech_fin;
            ViewBag.idCategoria = idCategoria;
            ViewBag.idSubCat = idSubCat;
            ViewBag.idRegion = idRegion;
        


            var lista = from obj in db.Eventos
                        where (obj.estado.Contains("Activo") == true)
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

 

            lista = lista.OrderBy(s => s.fecha_inicio);

            ViewBag.Cant = lista.Count();
          

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
            



            List<Eventos> listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null)).ToList();
            ViewBag.ListaDestacados = listaDestacados;

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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
    }
}