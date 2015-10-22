﻿using System;
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
                if (listaCategoria == null) return;
                else
                    for (int i = 0; i < listaCategoria.Count; i++)
                    {
                        borrar(listaCategoria[i].idCategoria);
                        db.Entry(listaCategoria[i]).State = EntityState.Modified;
                        listaCategoria[i].activo = 0;
                        db.SaveChanges();
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
    }

    
}