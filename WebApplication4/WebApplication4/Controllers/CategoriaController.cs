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

        public ActionResult Delete(int id)
        {
            Categoria categoria = db.Categoria.Find(id);            
            db.Entry(categoria).State = EntityState.Modified;
            categoria.activo = 0;
            db.SaveChanges();
            //return RedirectToAction("Index", "Evento");
            return View("Index");
        }
    }

    
}