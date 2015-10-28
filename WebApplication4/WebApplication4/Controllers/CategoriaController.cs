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

        public ActionResult Delete2(int id)
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

        public ActionResult Delete(string categoria)
        {
            //if (regalo == "" || regalo == null) return View("Index");
            int idQ = int.Parse(categoria);
            Categoria categoriaM = db.Categoria.Find(idQ);
            //db.Regalo.Remove(regalo);
            db.Entry(categoriaM).State = EntityState.Modified;
            categoriaM.activo = 0;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            borrar(idQ); 
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

        //probando
        /*private List<int> borrar(int id)
        {
            List<Categoria> listaCategoria = null;
            while (true)
            {
                listaCategoria = db.Categoria.Where(c => c.idCatPadre == id).ToList();
                if (listaCategoria.Count == 0) return;
                else
                {
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
        }*/


        private List<int> sacaDependientes(int id){
            List<int> lista = null;

            return lista;
        }

        public ActionResult Edit(string categoria)
        {            
            int id = int.Parse(categoria);
            ViewBag.id = id;
            TempData["codigo"] = id;
            Session["categoria"] = db.Categoria.Find(id);

            List<Categoria> listaCat = db.Categoria.Where(c => c.activo == 1).ToList();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre");

            List<int> ids = sacaDependientes(id);
            return View("Edit");
        }

        public ActionResult Edit2(int id)
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
                List<Categoria> cat = db.Categoria.Where(c => c.idCategoria == model.idCatPadre).ToList();
                categoria.nivel = cat[0].nivel+1;
                db.SaveChanges();
                return RedirectToAction("Index", "Categoria");
            }
            return RedirectToAction("Index", "Categoria");
        }

        //no me mires
        private void activar(int id)
        {
            List<Categoria> listaCategoria = null;
            while (true)
            {
                listaCategoria = db.Categoria.Where(c => c.idCatPadre == id).ToList();
                if (listaCategoria.Count == 0) return;
                else
                {
                    for (int i = 0; i < listaCategoria.Count; i++)
                    {
                        db.Entry(listaCategoria[i]).State = EntityState.Modified;
                        listaCategoria[i].activo = 1;
                        db.SaveChanges();
                        activar(listaCategoria[i].idCategoria);
                    }
                    return;
                }

            }
        }


        public ActionResult ActivateTree(string categoria)
        {
            //if (regalo == "" || regalo == null) return View("Index");
            int idQ = int.Parse(categoria);
            Categoria categoriaM = db.Categoria.Find(idQ);
            if (categoriaM.activo == 0)
            {
                //db.Regalo.Remove(regalo);
                db.Entry(categoriaM).State = EntityState.Modified;
                categoriaM.activo = 1;
                db.SaveChanges();
                //return RedirectToAction("Index", "Evento");
                activar(idQ);
            }
            return View("Index");
        }


        public ActionResult Activate(string categoria)
        {
            //if (regalo == "" || regalo == null) return View("Index");
            int idQ = int.Parse(categoria);
            Categoria categoriaM = db.Categoria.Find(idQ);
            if (categoriaM.activo == 0) {
                //db.Regalo.Remove(regalo);
                db.Entry(categoriaM).State = EntityState.Modified;
                categoriaM.activo = 1;
                db.SaveChanges();
                //return RedirectToAction("Index", "Evento");
            }            
            return View("Index");
        }
    }

    
}