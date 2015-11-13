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
    public partial class EventoController : Controller
    {

        [HttpPost]
        public ActionResult ReservaOrganizador(ReservaOrganizadorModel model){

            using (var context = new inf245netsoft())
            {
                try
                {

                    for (int i = 0; i < model.idFuncion.Length; i++)if(model.eliminar[i])
                    {
                        if (model.idAsiento[i] < 0)
                        {
                            var ZXF = context.ZonaxFuncion.Find(model.idFuncion[i], model.idZona[i]);
                            ZXF.cantReservaOrganizador--;
                            ZXF.cantLibres++;
                        }
                        else
                        {
                            var asientoReal = context.AsientosXFuncion.Find(model.idAsiento[i], model.idFuncion[i]);
                            asientoReal.estado = "libre";
                        }        
                            
                    }

                    context.SaveChanges();

                }
                catch (OptimisticConcurrencyException ex)
                {
                    TempData["tipo"] = "alert alert-warning";
                    TempData["message"] = "No se pudieron cancelar las reservas";
                    Redirect("~/Evento/verReservaOrganizador?evento=" + model.idEvento);
                }
            }

            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "Las reservas se cancelaron correctamente";
            return Redirect("~/Evento/verReservaOrganizador?evento=" + model.idEvento);
        }
        

        [HttpGet]
        public ActionResult verReservaOrganizador(string evento)
        {
            
            Eventos ev;

            if (evento == null || (ev = db.Eventos.Find(Int32.Parse(evento)))==null)
            {
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "No existe el evento";
                return Redirect("~/Evento");
            }

            var modelo = new ReservaOrganizadorModel();

            ViewBag.nombreEvento = ev.nombre;
            ViewBag.organizador = ev.Organizador.nombOrg;

            modelo.idEvento = ev.codigo;
            modelo.nameEvento = ev.nombre;
            List<int> zonas = new List<int>(0);
            List<int> funciones = new List<int>(0);
            List<int> idAsientos = new List<int>(0);
            List<string> nombresF = new List<string>(0);
            List<string> nombresZ = new List<string>(0);
            List<string> asiento = new List<string>(0);
            List<bool> elimina = new List<bool>(0);

            foreach (var funcion in ev.Funcion)
            {

                foreach (var asientoReal in funcion.AsientosXFuncion)
                {
                    if (asientoReal.estado == "RESERVAORGANIZADOR")
                    {
                        zonas.Add(asientoReal.Asientos.codZona);
                        nombresZ.Add(asientoReal.Asientos.ZonaEvento.nombre);
                        funciones.Add(funcion.codFuncion);
                        nombresF.Add(funcion.fecha + "");
                        asiento.Add(asientoReal.Asientos.fila + " " + asientoReal.Asientos.columna);
                        idAsientos.Add( asientoReal.Asientos.codAsiento );
                        elimina.Add(false);
                    }
                }

                foreach (var unidad in funcion.ZonaxFuncion) if (!unidad.ZonaEvento.tieneAsientos)
                    {
                        int tot = unidad.cantReservaOrganizador;
                        for (int i = 0; i < tot; i++)
                        {
                            zonas.Add(unidad.ZonaEvento.codZona);
                            funciones.Add(funcion.codFuncion);
                            asiento.Add("No tiene asientos");
                            nombresZ.Add(unidad.ZonaEvento.nombre);
                            idAsientos.Add(-1);
                            nombresF.Add(funcion.fecha + "");
                            elimina.Add(false);
                        }
                    }

            }

            modelo.idZona = zonas.ToArray();
            modelo.idFuncion = funciones.ToArray();
            modelo.AsientoXFuncion = asiento.ToArray();
            modelo.eliminar = elimina.ToArray();
            modelo.nameFuncion = nombresF.ToArray();
            modelo.nameZona = nombresZ.ToArray();
            modelo.idAsiento = idAsientos.ToArray();



            return View("ReservaOrganizador", modelo);
        }






        public string reservarOrganizador(PaqueteEntradas paquete)
        {
            try
            {
                using (var context = new inf245netsoft())
                {
                    try
                    {

                        if (paquete.tieneAsientos)
                        {
                            for (int i = 0; i < paquete.cantEntradas; i++)
                            {
                                int col = paquete.columnas[i];
                                int fil = paquete.filas[i];
                                List<Asientos> listasiento = context.Asientos.Where(x => x.codZona == paquete.idZona && x.fila == fil && x.columna == col).ToList();
                                AsientosXFuncion actAsiento = context.AsientosXFuncion.Find(listasiento.First().codAsiento, paquete.idFuncion);
                                actAsiento.estado = "RESERVAORGANIZADOR";
                            }
                        }
                        else
                        {
                            ZonaxFuncion ZXF = context.ZonaxFuncion.Find(paquete.idFuncion, paquete.idZona);
                            if (ZXF.cantLibres < paquete.cantEntradas) return "No hay suficientes entradas";
                            ZXF.cantLibres -= paquete.cantEntradas;
                            ZXF.cantReservaOrganizador += paquete.cantEntradas;
                        }
                        context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        return "No se pudieron reservar los asientos, alguien más ya lo hizo.";
                    }

                }
            }
            catch (Exception ex)
            {
                return "Ocurrio un error inesperado.";
            }
            //Funciones Utilitarias necesarias
            //BuscarEntradasLeQuedan( User , Funcion )
            return "Ok";

        }




	}
}