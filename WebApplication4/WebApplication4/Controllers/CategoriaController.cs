using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class CategoriaController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: Categoria
        public ActionResult Index()
        {
            return View();
        }
        //no me mires
        private void borrar(int id)
        {
            List<Categoria> listaCategoria = null;
            while (true)
            {
                listaCategoria = db.Categoria.Where(c=>c.idCatPadre==id).ToList();
                if (listaCategoria.Count == 0) return;
                else {
                    for (int i = 0; i < listaCategoria.Count; i++)
                    {
                        db.Entry(listaCategoria[i]).State = EntityState.Modified;
                        listaCategoria[i].activo = 0;
                        db.SaveChanges();
                        borrar(listaCategoria[i].idCategoria);
                    }
                    return;
                }                    

            }
        }

        public ActionResult Delete(int id)
        {            
            //borrar categoría padre
            Categoria categoria = db.Categoria.Find(id);
            db.Entry(categoria).State = EntityState.Modified;
            categoria.activo = 0;
            db.SaveChanges();
            //borrar arbol de la categoría padre
            borrar(id);                        
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }

        public ActionResult Search(CategoriaSearchModel categoria)
        {
            if (ModelState.IsValid)
            {
                List<Categoria> listaCat = db.Categoria.AsNoTracking().Where(c => c.nombre.StartsWith(categoria.nombre) && c.activo ==1).ToList();
                if (listaCat != null) TempData["ListaC"] = listaCat;
                else TempData["ListaC"] = null;
                return RedirectToAction("Index", "Categoria");
            }
            TempData["ListaC"] = null;
            return RedirectToAction("Index", "Categoria");
        }
        public ActionResult Edit(int id)
        {
            ViewBag.id = id;
            TempData["codigo"] = id;
            return View("Edit");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditRegister(CategoriaModel model)
        {
            if (ModelState.IsValid)
            {
                var o = ViewBag.id;
                Categoria categoria = db.Categoria.Find(TempData["codigo"]);
                db.Entry(categoria).State = EntityState.Modified;
                categoria.nombre = model.nombre;
                categoria.descripcion = model.descripcion;
                categoria.idCatPadre = model.idCatPadre;
                db.SaveChanges();
                return RedirectToAction("Index", "Categoria");
            }
            return RedirectToAction("Index", "Categoria");
        }
    }

    
}