using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using System.Data.Entity;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Core;

namespace WebApplication4.Controllers
{
    [Authorize(Roles = "Promotor")]
    public partial class EventoController : Controller
    {

        /*
         *POSTERGAR EVENTO 
         * 
        */
        [HttpGet]
        public ActionResult PostergarEvento(string evento)
        {
            if (String.IsNullOrEmpty(evento))
            {
                return RedirectToAction("Index2", "Home");
            }
            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();
            ViewBag.nombreEvento = queryEvento.nombre;
            int idOrganizador = (int)queryEvento.idOrganizador;
            ViewBag.idEvento = evento;
            ViewBag.organizadorEvento = db.Organizador.Where(c => c.codOrg == idOrganizador).First().nombOrg;
            ViewBag.listaFunciones = db.Funcion.Where(c => c.codEvento == id && c.estado != "CANCELADO").ToList();

            return View();
        }

        [HttpPost]
        public ActionResult PostergarEvento(PostergarModel evento)
        {

            Funcion funcionAPostergar = db.Funcion.Where(c => (c.codFuncion == evento.idFuncion)).First();

            if (evento.proximaFecha <= funcionAPostergar.fecha || evento.proximaFecha <= DateTime.Now)
            {
                //TempData["tipo"] = "alert alert-danger";
                //TempData["message"]="No puede elegir una fecha anterior";
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.StatusDescription = "No puede elegir una fecha anterior";
                return PostergarEvento(evento.idEvento+"");
            }

            db.Entry(funcionAPostergar).State = EntityState.Modified;

            funcionAPostergar.fecha = evento.proximaFecha;
            funcionAPostergar.horaIni = evento.proximaHora;
            funcionAPostergar.estado = "POSTERGADO";
            funcionAPostergar.fechaPostergado = DateTime.Now;//se guarda la fechaHora del acto de postergar

            int id = evento.idEvento;
            
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();

            db.Entry(queryEvento).State = EntityState.Modified;
            queryEvento.hanPostergado = true;
            db.SaveChanges();
            ObtenerFechaFin(evento.idEvento);
            EmailController.EnviarCorreoPostergarcionFuncion(evento.idFuncion);
            ViewBag.nombreEvento = queryEvento.nombre;
            int idOrganizador = (int)queryEvento.idOrganizador;
            ViewBag.idEvento = "" + id;
            ViewBag.organizadorEvento = db.Organizador.Where(c => c.codOrg == idOrganizador).First().nombOrg;
            ViewBag.listaFunciones = db.Funcion.Where(c => c.codEvento == id && c.estado != "CANCELADO").ToList();            
            
            return View();
        }


        /*
         *CANCELAR EVENTO 
         * 
        */

        [HttpGet]
        public ActionResult CancelarEvento(string evento)
        {

            int id = int.Parse(evento);
            Eventos queryEvento = db.Eventos.Where(c => c.codigo == id).First();

            List<Funcion> listaFunciones = db.Funcion.Where(c => c.codEvento == id && c.estado != "CANCELADO").ToList();

            CancelarModel cancelarEvento = new CancelarModel();
            cancelarEvento.idEvento = id;
            cancelarEvento.nombreEvento = queryEvento.nombre;
            cancelarEvento.organizador = db.Organizador.Where(c => c.codOrg == queryEvento.idOrganizador).First().nombOrg;

            var auxlistIdFuncion = new List<int>(0);
            var auxlistDateFuncion = new List<DateTime>(0);
            var auxlistBool = new List<Boolean>(0);

            for (int i = 0; i < listaFunciones.Count(); i++)
            {
                auxlistBool.Add(false);
                auxlistDateFuncion.Add((DateTime)listaFunciones[i].horaIni);
                auxlistIdFuncion.Add(listaFunciones[i].codFuncion);
            }

            cancelarEvento.listDateFuncion = auxlistDateFuncion.ToArray();
            cancelarEvento.listIdFuncion = auxlistIdFuncion.ToArray();
            cancelarEvento.seCancela = auxlistBool.ToArray();



            return View("Cancelar", cancelarEvento);
        }

        [HttpPost]
        public ActionResult CancelarEvento(CancelarModel evento)
        {
            if (evento.fechaRecojo < DateTime.Today)
            {
                ModelState.AddModelError("fechaRecojo", "Elija una fecha posterior al día de hoy");
            }

            int cnt = 0;
            if (evento.seCancela != null)
                for (int i = 0; i < evento.seCancela.Count(); i++)
                    if (evento.seCancela[i]) cnt++;

            if (cnt == 0)
            {
                ModelState.AddModelError("organizador", "Debe elegir un evento a cancelar");
            }

            if (ModelState.IsValid)
            {
                Eventos eventoACancelar = db.Eventos.Find(evento.idEvento);
                db.Entry(eventoACancelar).State = EntityState.Modified;
                eventoACancelar.hanCancelado = true;
                eventoACancelar.estado = "Cancelado";
                db.SaveChanges();

                for (int i = 0; i < evento.seCancela.Count(); i++)
                    if (evento.seCancela[i])
                    {
                        int idF = evento.listIdFuncion[i];
                        Funcion f = db.Funcion.Where(c => c.codFuncion == idF).First();

                        db.Entry(f).State = EntityState.Modified;
                        f.estado = "CANCELADO";
                        f.motivoCambio = evento.motivo;
                        f.FechaDevolucion = evento.fechaRecojo;
                        f.cantDiasDevolucion = evento.diasRecojo;
                        db.SaveChanges();
                    }
                // Cambiamos la fecha fin del evento , pues se ha cancelado una funcion
                ObtenerFechaFin(evento.idEvento);

                TempData["message"] = "Se cancelar las funciones correctamente";
                TempData["tipo"] = "alert alert-success";
                return Redirect("~/Evento");
            }
            return CancelarEvento("" + evento.idEvento);
        }
	}
}