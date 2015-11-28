using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriaController : Controller
    {
        private inf245netsoft db = new inf245netsoft();
        // GET: Categoria
        public ActionResult Index()
        {
            //Session["Ina"] = null; 
            /*if(Session["Bus"] != null){
                Session["ListaC"]=Session["Bus"];
                Session["Bus"]=null;
                return View();
            }else{
                List<Categoria> listaCategoria = null;
                listaCategoria=db.Categoria.Where(c=>c.activo==1 && c.nivel!=0).ToList();
                Session["ListaC"] = listaCategoria;*/
            return View();
            //}
        }
        //no me mires
        private void borrar(int id)
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
                List<Categoria> listaCat = db.Categoria.AsNoTracking().Where(c => c.nombre.StartsWith(categoria.nombre) && c.activo == 1).ToList();
                if (listaCat != null) TempData["ListaC"] = listaCat;
                else TempData["ListaC"] = null;
                return RedirectToAction("Index", "Categoria");
            }
            TempData["ListaC"] = null;
            return RedirectToAction("Index", "Categoria");
        }

        //probando
        private List<int> sacaListaDependientes(int id)
        {
            List<int> listaID = new List<int>();
            List<Categoria> listaCategoria = null;
            while (true)
            {
                listaCategoria = db.Categoria.Where(c => c.idCatPadre == id).ToList();
                if (listaCategoria.Count == 0) return listaID;
                else
                {
                    for (int i = 0; i < listaCategoria.Count; i++)
                    {
                        listaID.Add(listaCategoria[i].idCategoria);
                        listaID.AddRange(sacaListaDependientes(listaCategoria[i].idCategoria));
                    }
                    return listaID;
                }

            }
        }


        private void sacaDependientes(List<Categoria> listaCat, int id)
        {
            List<int> lista = new List<int>();
            lista.AddRange(sacaListaDependientes(id));

            for (int i = 0; i < lista.Count; i++)
            {
                for (int j = 0; j < listaCat.Count; j++)
                {
                    if (listaCat[j].idCategoria == id)//Código chancho
                    {
                        listaCat.RemoveAt(j);
                        break;
                    }
                    if (listaCat[j].idCategoria == lista[i])
                    {
                        listaCat.RemoveAt(j);
                        break;
                    }
                }

            }
        }

        public ActionResult ViewIna(CategoriaViewInaModel categoria)
        {
            List<Categoria> listaCat;
            //Session["Bus"] = null;
            listaCat = db.Categoria.AsNoTracking().Where(c => c.activo == 0).ToList();
            if (listaCat != null) Session["Ina"] = listaCat;
            else Session["Ina"] = null;
            return RedirectToAction("Index", "Categoria");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterCategoria(CategoriaModel model)
        {
            if (ModelState.IsValid)
            {
                Categoria categoria = new Categoria();
                Categoria categoriaL = db.Categoria.ToList().Last();
                categoria.idCategoria = categoriaL.idCategoria + 1;
                categoria.nombre = model.nombre;
                categoria.activo = 1;
                categoria.descripcion = model.descripcion;
                categoria.idCatPadre = model.idCatPadre;
                List<Categoria> cat = db.Categoria.Where(c => c.idCategoria == model.idCatPadre).ToList();
                categoria.nivel = cat[0].nivel + 1;

                db.Categoria.Add(categoria);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Search2(string categoria)
        {
            List<Categoria> listaCat;
            if (categoria == "")
            {
                //listaReg = db.Regalo.AsNoTracking().Where(c => c.estado == true).ToList();
                Session["Bus"] = null;
                return RedirectToAction("Index", "Categoria");
            }
            listaCat = db.Categoria.AsNoTracking().Where(c => c.nombre.StartsWith(categoria) && c.activo == 1 && c.nivel != 0).ToList();
            if (listaCat != null) Session["Bus"] = listaCat;
            else Session["Bus"] = null;
            return RedirectToAction("Index", "Categoria");
        }

        public ActionResult Edit(string categoria)
        {
            //Session["Bus"] = null;
            int id = int.Parse(categoria);
            ViewBag.id = id;
            TempData["codigo"] = id;
            Categoria categ = db.Categoria.Find(id);
            Session["categoria"] = categ;
            CategoriaModel catM = new CategoriaModel();
            catM.idCatPadre = (int)categ.idCatPadre;

            List<Categoria> listaCat = db.Categoria.Where(c => c.activo == 1).ToList();
            ViewBag.CatID = new SelectList(listaCat, "idCategoria", "nombre", catM.idCatPadre);

            sacaDependientes(listaCat, id);
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
            //if (ModelState.IsValid)
            //{
            var o = ViewBag.id;
            Categoria categoria = db.Categoria.Find(TempData["codigo"]);
            db.Entry(categoria).State = EntityState.Modified;
            if (!String.IsNullOrEmpty(model.nombre)) categoria.nombre = model.nombre;
            if (!String.IsNullOrEmpty(model.descripcion)) categoria.descripcion = model.descripcion;
            if (model.idCatPadre != 0)
            {
                categoria.idCatPadre = model.idCatPadre;
                List<Categoria> cat = db.Categoria.Where(c => c.idCategoria == model.idCatPadre).ToList();
                categoria.nivel = cat[0].nivel + 1;
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Categoria");
            //}
            //return RedirectToAction("Index", "Categoria");
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
            if (categoriaM.activo == 0)
            {
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