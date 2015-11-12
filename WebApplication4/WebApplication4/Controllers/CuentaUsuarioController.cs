using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class Fechas
    {
        public static List<SelectListItem> Mes()
        {
            List<SelectListItem> lista = new List<SelectListItem> {
                new SelectListItem{Value="1",Text="Enero"},
                new SelectListItem{Value="2",Text="Febrero"},
                new SelectListItem{Value="3",Text="Marzo"},
                new SelectListItem{Value="4",Text="Abril"},
                new SelectListItem{Value="5",Text="Mayo"},
                new SelectListItem{Value="6",Text="Junio"},
                new SelectListItem{Value="7",Text="Julio"},
                new SelectListItem{Value="8",Text="Agosto"},
                new SelectListItem{Value="9",Text="Septiembre"},
                new SelectListItem{Value="10",Text="Octubre"},
                new SelectListItem{Value="11",Text="Noviembre"},
                new SelectListItem{Value="12",Text="Diciembre"}
            };
            return lista;
        }
        public static List<SelectListItem> Anio()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            DateTime hoy = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                lista.Add(new SelectListItem { Text = "" + (hoy.Year + i), Value = "" + (hoy.Year + i) });
            }
            return lista;
        }
    }

    [Authorize]
    public class CuentaUsuarioController : Controller
    {
        private inf245netsoft db = new inf245netsoft();

        //solo sirve para el primer caso del banco y tipo tarjeta. Luego uso otra funcion que retorna un json
        private Promociones CalculaMejorPromocionTarjeta(int codEvento, int idBanco, int tipoTarjeta)
        {
            try
            {
                //busco las promociones que se encuentren activas
                List<Promociones> promociones = db.Promociones.Where(c => c.codEvento == codEvento && c.codBanco == idBanco && c.codTipoTarjeta == tipoTarjeta && c.estado == true && c.fechaIni <= DateTime.Today && DateTime.Today <= DateTime.Today).ToList();
                promociones.Sort((a, b) => ((double)a.descuento).CompareTo((double)b.descuento));
                return promociones.Last();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ComprarEntrada()
        {
            if (Session["CarritoItem"] != null)
            {
                //saco el carrito del session
                List<CarritoItem> carrito = (List<CarritoItem>)Session["CarritoItem"];
                //lista de bancos
                List<Banco> bancos = db.Banco.ToList();
                ViewBag.Bancos = new SelectList(bancos, "codigo", "nombre");
                //lista de tarjetas
                List<TipoTarjeta> tipoTarjeta = db.TipoTarjeta.ToList();

                ViewBag.TipoTarjeta = new SelectList(tipoTarjeta, "idTipoTar", "nombre");
                List<Promociones> listaPromociones = new List<Promociones>();
                double total = 0;
                double descuento = 0;
                foreach (CarritoItem item in carrito)
                {
                    total += item.precio;
                    Promociones promocion = CalculaMejorPromocionTarjeta(item.idEvento, bancos.First().codigo, tipoTarjeta.First().idTipoTar);
                    if (promocion == null)
                    {
                        Promociones dummy = new Promociones();
                        dummy.codPromo = -1;
                        listaPromociones.Add(dummy);
                    }
                    else
                    {
                        descuento += item.precio * promocion.descuento.Value / 100;
                        listaPromociones.Add(promocion);
                    }
                }
                ViewBag.Descuento = descuento;
                ViewBag.Promociones = listaPromociones;
                ViewBag.Total = total;
                ViewBag.Pagar = total - descuento;
                ViewBag.Mes = Fechas.Mes();
                ViewBag.AnVen = Fechas.Anio();
                return View();
            }
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "No hay items en el carrito.";
            return RedirectToAction("MiCarrito");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ComprarEntrada(ComprarEntradaModel model)
        {
            if (ModelState.IsValid)
            {
                int idVenta = 0;
                DateTime hoy = DateTime.Today;
                CuentaUsuario cuenta = new CuentaUsuario();
                using (var context = new inf245netsoft())
                {
                    try
                    {
                        List<CarritoItem> carrito = (List<CarritoItem>)Session["CarritoItem"];
                        Ventas ve = new Ventas();
                        int cantidadEntradasTotales = carrito.Sum(c => c.cantidad);

                        if (Session["UsuarioLogueado"] != null)
                        {
                            cuenta = (CuentaUsuario)Session["UsuarioLogueado"];
                            ve.CuentaUsuario = cuenta;
                        }
                        else
                        {
                            ve.CuentaUsuario = db.CuentaUsuario.Find(MagicHelpers.AnonimoUniversal);
                        }
                        ve.fecha = DateTime.Now;
                        ve.cantAsientos = cantidadEntradasTotales;
                        //de todas maneras en la venta se registra el nombre, dni y tipo de documento del que esta comprando.
                        ve.cliente = model.Nombre;
                        ve.codDoc = model.Dni;
                        ve.Estado = MagicHelpers.Compra;
                        ve.tipoDoc = 1;
                        ve.montoEfectivoSoles = model.Importe;
                        ve.MontoTotalSoles = model.MontoPagar;
                        db.Ventas.Add(ve);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Trace.TraceInformation("Property: {0} Error: {1}",
                                                            validationError.PropertyName,
                                                            validationError.ErrorMessage);
                                }
                            }
                        }
                        //para cada item del carrito
                        for (int w = 0; w < carrito.Count; w++)
                        {
                            CarritoItem paquete = carrito[w];
                            //zona del evento
                            ZonaEvento zo = db.ZonaEvento.Find(paquete.idZona);
                            //en que perdiodo de venta estamos
                            PeriodoVenta per = db.PeriodoVenta.Where(c => c.codEvento == paquete.idEvento && c.fechaInicio <= hoy && c.fechaFin >= hoy).ToList().First();
                            PrecioEvento pr = db.PrecioEvento.Where(c => c.codZonaEvento == paquete.idZona && c.codPeriodoVenta == per.idPerVent).ToList().First();
                            //la venta x funcion requerida
                            VentasXFuncion vf = new VentasXFuncion();
                            vf.codVen = ve.codVen;
                            vf.cantEntradas = paquete.cantidad;
                            vf.codFuncion = paquete.idFuncion;
                            vf.Ventas = ve;
                            vf.Funcion = db.Funcion.Find(paquete.idFuncion);
                            vf.descuento = 0;
                            vf.subtotal = paquete.cantidad * pr.precio;
                            vf.total = paquete.cantidad * pr.precio;
                            db.VentasXFuncion.Add(vf);
                            db.SaveChanges();
                            //detalle de venta
                            DetalleVenta dt = new DetalleVenta();
                            dt.cantEntradas = paquete.cantidad;
                            dt.codFuncion = paquete.idFuncion;
                            dt.codPrecE = pr.codPrecioEvento;
                            dt.total = paquete.cantidad * pr.precio;
                            dt.entradasDev = 0;
                            dt.descTot = 0;
                            dt.codVen = vf.codVen;
                            db.DetalleVenta.Add(dt);
                            if (paquete.filas != null && paquete.filas.Count > 0) paquete.tieneAsientos = true;
                            db.SaveChanges();
                            //si tengo asientos, actualizo los asientos a ocupado
                            if (paquete.tieneAsientos)
                            {
                                for (int i = 0; i < paquete.cantidad; i++)
                                {
                                    int col = paquete.columnas[i];
                                    int fil = paquete.filas[i];
                                    List<Asientos> listasiento = context.Asientos.Where(x => x.codZona == paquete.idZona && x.fila == fil && x.columna == col).ToList();
                                    AsientosXFuncion actAsiento = context.AsientosXFuncion.Find(listasiento.First().codAsiento, paquete.idFuncion);
                                    actAsiento.estado = MagicHelpers.Ocupado;
                                    actAsiento.codDetalleVenta = dt.codDetalleVenta;
                                    actAsiento.PrecioPagado = pr.precio;
                                }
                            }
                            else
                            {
                                //si no tiene asientos es una zonax funcion
                                ZonaxFuncion ZXF = context.ZonaxFuncion.Find(paquete.idFuncion, paquete.idZona);
                                if (ZXF.cantLibres < paquete.cantidad)
                                {
                                    //genero una exception para detener la compra?
                                    throw new Exception();
                                }
                                else
                                    ZXF.cantLibres -= paquete.cantidad;
                            }
                            if (User.Identity.IsAuthenticated)
                            {//si es u usuario registrado le aumento los puntos que tiene
                                CuentaUsuario dbCuenta = db.CuentaUsuario.Find(cuenta.correo);
                                dbCuenta.puntos += db.Eventos.Find(paquete.idEvento).puntosAlCliente * paquete.cantidad;
                            }
                        }

                        db.SaveChanges();

                        context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException ex)
                    {
                        //hubo un problema con la compra, remuevo el item
                        if (idVenta != 0)
                        {
                            Ventas remover = db.Ventas.Find(idVenta);
                            db.Ventas.Remove(remover);
                        }
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = "Error en la compra.";
                        return View(model);
                    }
                }
                TempData["tipo"] = "alert alert-success";
                TempData["message"] = "Compra Realizada. Muchas Gracias.";
                //si toda la compra se procesa de manera correcta eliminamos los session
                Session["CarritoItem"] = null;
                Session["Carrito"] = null;
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ModificarDatos()
        {
            string correo = User.Identity.Name;
            CuentaUsuario cliente = db.CuentaUsuario.Where(c => c.correo == correo).First();
            EditClientModel client = new EditClientModel();
            client.apellido = cliente.apellido;
            client.codDoc = cliente.codDoc;
            client.direccion = cliente.direccion;
            client.fechaNac = (DateTime)cliente.fechaNac;
            client.nombre = cliente.nombre;
            client.telefono = cliente.telefono;
            client.telMovil = cliente.telMovil;
            client.tipoDoc = (int)cliente.tipoDoc;
            return View(client);
        }

        [HttpPost]
        public ActionResult ModificarDatos(EditClientModel model)
        {
            string correo = User.Identity.Name;
            CuentaUsuario cliente = db.CuentaUsuario.Where(c => c.correo == correo).First();
            cliente.apellido = model.apellido;
            cliente.codDoc = model.codDoc;
            cliente.direccion = model.direccion;
            cliente.fechaNac = model.fechaNac;
            cliente.nombre = model.nombre;
            cliente.telefono = model.telefono;
            cliente.telMovil = model.telMovil;
            cliente.tipoDoc = model.tipoDoc;
            db.SaveChanges();
            TempData["tipo"] = "alert alert-success";
            TempData["message"] = "Datos Actualizados Exitosamente";
            return RedirectToAction("MiCuenta");
        }

        [HttpGet]
        public ActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarContrasena(CambiarContrasenaModel model)
        {
            string correo = User.Identity.Name;
            CuentaUsuario cliente = db.CuentaUsuario.Where(c => c.correo == correo).First();
            /*var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);*/

            if (ModelState.IsValid)
            {
                if (cliente.contrasena == model.Contrasena)
                {
                    if (model.NuevaContrasena == model.RNuevaContrasena && model.NuevaContrasena != cliente.contrasena)
                    {
                        cliente.contrasena = model.NuevaContrasena;
                        //user.PasswordHash = model.NuevaContrasena;
                        db.SaveChanges();
                        TempData["tipo"] = "alert alert-success";
                        TempData["message"] = "Contraseña cambiada Exitosamente";
                    }
                    return RedirectToAction("MiCuenta");
                }
            }
            TempData["tipo"] = "alert alert-success";
            TempData["message"] = "Contraseña cambiada Exitosamente";
            return View(model);
        }

        [HttpGet]
        public ActionResult CambiarCorreo()
        {
            string correo = User.Identity.Name;
            CambiarCorreoModel model = new CambiarCorreoModel();
            model.Email = correo;
            return View(model);
        }

        private bool YaExiste(string correo)
        {
            bool cuentausuario = db.CuentaUsuario.Any(c => c.correo == correo);
            bool aspNetuser = db.AspNetUsers.Any(c => c.Email == correo);
            return cuentausuario && aspNetuser;
        }

        [HttpPost]
        public ActionResult CambiarCorreo(CambiarCorreoModel model)
        {
            if (ModelState.IsValid)
            {
                if (YaExiste(model.Email))
                {
                    ModelState.AddModelError("Email", "Correo en uso.");
                    return View(model);
                }
                CuentaUsuario cuenta = db.CuentaUsuario.Where(c => c.correo == User.Identity.Name).First();
                cuenta.correo = model.Email;
                db.SaveChanges();
                AspNetUsers aspCuenta = db.AspNetUsers.Where(c => c.Email == User.Identity.Name).First();
                aspCuenta.Email = model.Email;
                db.SaveChanges();
                return RedirectToAction("MiCuenta");
            }
            return View(model);
        }

        public ActionResult BuscaCliente()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(ClienteSearchModel cliente)
        {
            if (ModelState.IsValid)
            {
                List<CuentaUsuario> listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == cliente.tipoDoc && c.codDoc == cliente.codDoc && c.estado == true && c.codPerfil == 1).ToList();
                if (listacl != null) TempData["ListaCL"] = listacl;
                else TempData["ListaCL"] = null;
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            TempData["ListaCL"] = null;
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public ActionResult Search2(string usuario, string tipo)
        {
            //if (tipo == "")
            //{
            //    Session["ListaCL"] = null;
            //    return RedirectToAction("BuscaCliente", "CuentaUsuario");
            //}
            int ti = 0;
            if (tipo != "") ti = int.Parse(tipo);
            //if (ti == 0)
            //{
            //    Session["ListaCL"] = null;
            //    return RedirectToAction("BuscaCliente", "CuentaUsuario");
            //}
            if (usuario == "" || usuario == null)
            {
                Session["ListaCL"] = null;
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            List<CuentaUsuario> listacl;
            if (ti == 0) listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc.Contains(usuario) && c.estado == true && c.codPerfil == 1 && c.puntos > 0).ToList();
            else listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == ti && c.codDoc.Contains(usuario) && c.estado == true && c.codPerfil == 1 && c.puntos > 0).ToList();
            if (listacl != null) Session["ListaCL"] = listacl;
            else Session["ListaCL"] = null;
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public ActionResult SearchReserva(string usuario, string tipo)
        {
            //if (tipo == "")
            //{
            //    Session["ReservaBusca"] = null;
            //    return RedirectToAction("BuscaReserva", "CuentaUsuario");
            //}
            int ti = 0;
            if (tipo != "") ti = int.Parse(tipo);
            //if (ti == 0)
            //{
            //    Session["ReservaBusca"] = null;
            //    return RedirectToAction("BuscaReserva", "CuentaUsuario");
            //}
            if (usuario == "" || usuario == null)
            {
                Session["ReservaBusca"] = null;
                return RedirectToAction("BuscaReserva", "CuentaUsuario");
            }
            //List<CuentaUsuario> listacl;
            //if (ti == 0) listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc.Contains(usuario) && c.estado == true && c.codPerfil == 1).ToList();
            //else listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == ti && c.codDoc.Contains(usuario) && c.estado == true && c.codPerfil == 1).ToList();
            //if (listacl == null) return RedirectToAction("BuscaReserva", "CuentaUsuario");

            List<Ventas> listareservas = new List<Ventas>();
            //for (int i = 0; i < listacl.Count; i++)
            //{
            //    string us = listacl[i].usuario;
            //    List<Ventas> lv = db.Ventas.Where(c => c.cliente == us && c.Estado == "Reservado").ToList();
            //    for (int j = 0; j < lv.Count; j++)
            //    {
            //        listareservas.Add(lv[j]);
            //    }
            //}

            if (ti == 0) listareservas = db.Ventas.AsNoTracking().Where(c => c.codDoc.Contains(usuario) && c.Estado == "Reservado").ToList();
            else listareservas = db.Ventas.AsNoTracking().Where(c => c.tipoDoc == ti && c.codDoc.Contains(usuario) && c.Estado == "Reservado").ToList();
            if (listareservas == null) return RedirectToAction("BuscaReserva", "CuentaUsuario");

            List<VentasXFuncion> listaRxF = new List<VentasXFuncion>();
            for (int i = 0; i < listareservas.Count; i++)
            {
                int cov = listareservas[i].codVen;
                List<VentasXFuncion> lvf = db.VentasXFuncion.Where(c => c.codVen == cov).ToList();
                for (int j = 0; j < lvf.Count; j++)
                {
                    listaRxF.Add(lvf[j]);
                }
            }
            if (listaRxF != null) Session["ReservaBusca"] = listaRxF;
            else Session["ReservaBusca"] = null;
            return RedirectToAction("BuscaReserva", "CuentaUsuario");
        }

        public ActionResult Politicas()
        {
            return View();
        }

        public ActionResult MiCuenta()
        {
            return View();
        }

        public ActionResult Asignacion()
        {
            //if (Session["nError"] != null)
            //{
            //    int ner=(int)Session["nError"];
            //    if (Session["ErrorAsignacion"] != null)
            //    {
            //        if (ner > 1) {
            //            Session["ErrorAsignacion"] = null;
            //            Session["nError"] = 0;
            //        }
            //        Session["nError"] = ner + 1;
            //    }
            //}
            return View();
        }

        public ActionResult MisReservas()
        {
            return View();
        }

        public ActionResult DeleteReserva(int codE, int codF, int codEv, int codZ)
        {
            Ventas v = db.Ventas.Find(codE);
            VentasXFuncion vxf = db.VentasXFuncion.Find(codE, codF);
            ZonaEvento zo = db.ZonaEvento.Find(codZ);
            if (zo.tieneAsientos == true)
            {
                //db.VentasXFuncion.Remove(vxf);
                vxf.cantEntradas = 0;
                List<DetalleVenta> ldt = db.DetalleVenta.Where(c => c.codFuncion == codF && c.codVen == codE).ToList();
                DetalleVenta dt = ldt.First();
                List<AsientosXFuncion> laf = db.AsientosXFuncion.Where(c => c.codDetalleVenta == dt.codDetalleVenta && c.codFuncion == codF).ToList();
                for (int i = 0; i < laf.Count; i++)
                {
                    laf[i].estado = "libre";
                }
                v.Estado = "Anulado";
                //db.VentasXFuncion.Remove(vxf);
                db.SaveChanges();
            }
            else
            {
                ZonaxFuncion zxf = db.ZonaxFuncion.Find(codF, codZ);
                //db.Entry(zxf).State = EntityState.Modified;
                v.Estado = "Anulado";
                zxf.cantLibres += (int)v.cantAsientos;
                vxf.cantEntradas = 0;
                db.SaveChanges();
            }
            return RedirectToAction("MisReservas", "CuentaUsuario");
        }

        public JsonResult RegistraPoliticas(string dur, string mx, string mt, string ra, string mE, string hr)
        {
            int m1, m2, m3, m4, m5, m6;
            DateTime h6;
            string me1 = "1.Falta Ingresar Valores\n", me2 = " 3.Falta Ingresar Valores\n", me3 = " 4.Falta Ingresar Valores\n", me4 = " 5.Falta Ingresar Valores\n", me5 = " 6.Falta Ingresar Valores", me6 = " 2.Falta Ingresar Valores\n";
            if (int.TryParse(dur, out m1) == true)
            {
                int val = int.Parse(dur);
                if (val > 0)
                {
                    int t = 1;
                    Politicas p = db.Politicas.Find(t);
                    db.Entry(p).State = EntityState.Modified;
                    p.valor = val;
                    db.SaveChanges();
                    db.Entry(p).State = EntityState.Detached;
                    me1 = "1.Completado\n";
                }
                else
                {
                    me1 = " 1.Error Negativo\n";
                }
            }
            if (dur == "e")
            {
                int t = 1;
                Politicas p = db.Politicas.Find(t);
                db.Entry(p).State = EntityState.Modified;
                p.valor = null;
                db.SaveChanges();
            }
            if (int.TryParse(mx, out m2) == true)
            {
                int val1 = int.Parse(mx);
                if (val1 > 0)
                {
                    int t = 2;
                    Politicas p = db.Politicas.Find(t);
                    db.Entry(p).State = EntityState.Modified;
                    p.valor = val1;
                    db.SaveChanges();
                    db.Entry(p).State = EntityState.Detached;
                    me2 = " 3.Completado\n";
                }
                else
                {
                    me2 = " 3.Error Negativo\n";
                }
            }
            if (int.TryParse(mt, out m3) == true)
            {
                int val2 = int.Parse(mt);
                if (val2 > 0)
                {
                    int t = 3;
                    Politicas p = db.Politicas.Find(t);
                    db.Entry(p).State = EntityState.Modified;
                    p.valor = val2;
                    db.SaveChanges();
                    db.Entry(p).State = EntityState.Detached;
                    me3 = " 4.Completado\n";
                }
                else
                {
                    me3 = " 4.Error Negativo\n";
                }

            }
            if (int.TryParse(ra, out m4) == true)
            {
                int val3 = int.Parse(ra);
                if (val3 > 0)
                {
                    int t = 5;
                    Politicas p = db.Politicas.Find(t);
                    db.Entry(p).State = EntityState.Modified;
                    p.valor = val3;
                    db.SaveChanges();
                    db.Entry(p).State = EntityState.Detached;
                    me4 = " 5.Completado\n";
                }
                else
                {
                    me4 = " 5.Error Negativo\n";
                }
            }
            if (int.TryParse(mE, out m5) == true)
            {
                int val5 = int.Parse(mE);
                if (val5 > 0)
                {
                    int t = 7;
                    Politicas p = db.Politicas.Find(t);
                    db.Entry(p).State = EntityState.Modified;
                    p.valor = val5;
                    db.SaveChanges();
                    db.Entry(p).State = EntityState.Detached;
                    me5 = " 6.Completado\n";
                }
                else
                {
                    me5 = " 6.Error Negativo\n";
                }
            }
            if (DateTime.TryParse(hr, out h6) == true)
            {
                DateTime hr6 = DateTime.Parse(hr);
                HoraReserva h = db.HoraReserva.Find(6);
                db.Entry(h).State = EntityState.Modified;
                h.hora = hr6;
                db.SaveChanges();
                me6 = " 2.Completado\n";
            }
            if (hr == "e")
            {
                HoraReserva h = db.HoraReserva.Find(6);
                db.Entry(h).State = EntityState.Modified;
                h.hora = null;
                db.SaveChanges();
            }
            string mensaje = me1 + me6 + me2 + me3 + me4 + me5;
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegistraTolerancia(String tolerancia)
        {
            string mensaje = "Ingrese datos";
            if (tolerancia == "" || tolerancia == null) return Json(mensaje, JsonRequestBehavior.AllowGet);
            //double m1;
            int m1;
            string me = "Error";
            mensaje = me;
            if (int.TryParse(tolerancia, out m1) == false) return Json(mensaje, JsonRequestBehavior.AllowGet);
            //double m = double.Parse(tolerancia);
            //int val = (int)m;
            int val = int.Parse(tolerancia);
            if (val > 0)
            {
                int t = 4;
                Politicas p = db.Politicas.Find(t);
                db.Entry(p).State = EntityState.Modified;
                p.valor = val;
                db.SaveChanges();
                db.Entry(p).State = EntityState.Detached;
                mensaje = "Registro completo";
            }
            else
            {
                mensaje = "Error numero negativo";
            }
            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscaReserva()
        {
            return View();
        }

        public JsonResult DeleteTurno(string turno, string fecha, string horai)
        {
            string m1;
            CuentaUsuario vend;
            int cpv;
            if (Session["vendAsig"] != null)
            {
                vend = (CuentaUsuario)Session["vendAsig"];
                m1 = vend.usuario;
            }
            else
            {
                return Json("Seleccione un vendedor", JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Asignacion", "CuentaUsuario");
            }
            List<TurnoSistema> lts = db.TurnoSistema.Where(c => c.horIni == horai).ToList();
            int cs = lts.First().codTurnoSis;
            DateTime dt1 = DateTime.Parse(fecha);
            cpv = int.Parse(turno);
            List<Turno> ltur = db.Turno.Where(s => s.codPuntoVenta == cpv && s.codTurnoSis == cs && s.usuario == m1 && s.fecha == dt1).ToList();
            Turno tur = ltur.First();
            db.Turno.Remove(tur);
            db.SaveChanges();
            DateTime hoy = DateTime.Now;
            List<Turno> listatuvend = db.Turno.AsNoTracking().Where(c => c.usuario == m1 && c.fecha > hoy).ToList();
            Session["ListaTurnoVendedor"] = listatuvend;
            return Json("Turno Eliminado", JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegistrarAsignacion(string turno, string punto, string idV, string ini, string fin)
        {
            //if (Session["ErrorAsignacion"] != null) Session["ErrorAsignacion"] = null;
            string m1;
            CuentaUsuario vend;
            int cpv;
            DateTime dt1 = DateTime.Parse(ini);
            DateTime dt2 = DateTime.Parse(fin);
            DateTime di = dt1;
            DateTime dai = dt1;
            int idt = int.Parse(turno);
            int idp = int.Parse(punto);
            TimeSpan ts = dt2.Subtract(dt1);
            int nd = (int)ts.Days;
            nd = nd + 1;
            int idPol = 5;
            int idPol2 = 7;
            int limite = (int)db.Politicas.Find(idPol).valor;
            int limite2 = (int)db.Politicas.Find(idPol2).valor;
            if (dt1 <= DateTime.Now) return Json("la fecha debe ser superior de hoy", JsonRequestBehavior.AllowGet);
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            if (nd > limite) return Json("No puedo asignar a la vez mas de " + limite + " turnos de manera seguida", JsonRequestBehavior.AllowGet);
            //int cruce = 0;            
            for (int j = 0; j < nd; j++)
            {
                List<Turno> ltur = db.Turno.Where(c => c.codPuntoVenta == idp && c.codTurnoSis == idt && di == c.fecha).ToList();
                if (ltur.Count + 1 > limite2)
                {
                    //Session["nError"] = 1;
                    //TempData["ErrorAsignacion"] = "Cruce con el usuario " + ltur.First().usuario + " para el dia " + di;
                    //return RedirectToAction("Asignacion", "CuentaUsuario");
                    string mensaje = "No puede haber mas de " + limite2 + " vendedores para el dia " + di.ToString("dd/MM/yyyy");

                    return Json(mensaje, JsonRequestBehavior.AllowGet);
                }
                di = di.AddDays(1);
            }
            //int cruce1 = 0;
            List<Turno> ltur2 = db.Turno.Where(c => c.codPuntoVenta == idp && c.codTurnoSis == idt && dai == c.fecha && c.usuario == idV).ToList();
            if (ltur2.Count > 0)
            {
                return Json("Este usuario ya tiene asignado un turno para esta fecha , punto de venta y hora", JsonRequestBehavior.AllowGet);
            }
            List<Turno> ltur3 = db.Turno.Where(c => c.codTurnoSis == idt && dai == c.fecha && c.usuario == idV).ToList();
            if (ltur3.Count > 0)
            {
                return Json("Este usuario ya tiene asignado un turno para esta fecha y hora en otro punto de venta", JsonRequestBehavior.AllowGet);
            }
            for (int j = 0; j < nd; j++)
            {
                Turno ntur = new Turno();
                ntur.codPuntoVenta = idp;
                ntur.codTurnoSis = idt;
                ntur.estado = "Pendiente";
                ntur.estadoCaja = "Pendiente";
                ntur.fecha = dai;
                ntur.MontoFinDolares = 0;
                ntur.MontoFinSoles = 0;
                ntur.MontoInicioDolares = 0;
                ntur.MontoInicioSoles = 0;
                ntur.usuario = idV;
                db.Turno.Add(ntur);
                db.SaveChanges();
                dai = dai.AddDays(1);
            }
            DateTime hoy = DateTime.Now;
            List<Turno> listatuvend = db.Turno.AsNoTracking().Where(c => c.usuario == idV && c.fecha > hoy).ToList();
            Session["ListaTurnoVendedor"] = listatuvend;
            return Json("Registro Correcto", JsonRequestBehavior.AllowGet);
        }

        public ActionResult MisCompras()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult MiCarrito()
        {
            if (Session["Carrito"] != null)
            {
                List<PaqueteEntradas> carrito = (List<PaqueteEntradas>)Session["Carrito"];
                List<CarritoItem> item = new List<CarritoItem>();
                foreach (PaqueteEntradas paquete in carrito)
                {
                    Eventos evento = db.Eventos.Find(paquete.idEvento);
                    PeriodoVenta periodo = db.PeriodoVenta.Where(c => c.codEvento == paquete.idEvento && c.fechaInicio <= DateTime.Today && DateTime.Today <= c.fechaFin).First();
                    CarritoItem cItem = new CarritoItem();
                    cItem.idEvento = paquete.idEvento;
                    cItem.idFuncion = paquete.idFuncion;
                    cItem.idZona = paquete.idZona;
                    cItem.nombreEvento = db.Eventos.Find(paquete.idEvento).nombre;
                    Funcion funcion = db.Funcion.Find(paquete.idFuncion);
                    cItem.fecha = (DateTime)funcion.fecha;
                    cItem.hora = (DateTime)funcion.horaIni;
                    cItem.zona = db.ZonaEvento.Find(paquete.idZona).nombre;
                    cItem.precio = (double)db.PrecioEvento.Where(c => c.codZonaEvento == paquete.idZona && c.codPeriodoVenta == periodo.idPerVent).First().precio * paquete.cantEntradas;
                    cItem.filas = paquete.filas;
                    cItem.columnas = paquete.columnas;
                    cItem.tieneAsientos = paquete.tieneAsientos;
                    cItem.cantidad = paquete.cantEntradas;
                    item.Add(cItem);
                }
                Session["CarritoItem"] = item;
                ViewBag.Carrito = item;
            }
            return View();
        }

        public ActionResult ReporteCliente()
        {
            return View();
        }

        public ActionResult Distritos()
        {

            return View();

        }

        public ActionResult Punto()
        {
            return View();
        }

        public ActionResult ReportePdf()
        {
            //List<WebApplication4.Models.CuentaUsuario> listaCliente = (List<WebApplication4.Models.CuentaUsuario>)TempData["ListaPU"];
            //Document document = new Document();
            //PdfWriter.GetInstance(document, new FileStream("F://ReporteVentas.pdf", FileMode.OpenOrCreate));
            //document.Open();
            //DateTime d1 = DateTime.Now;
            //document.Add(new Paragraph(""));
            //document.Add(new Paragraph("                                                            Reporte de Clientes"));
            //document.Add(new Paragraph("            Fecha:               " + d1.Date + "                     "));
            //document.Add(new Paragraph("                     Usuario        Nombre y Apellido             Codigo Documento       Puntos"));
            //for (int i = 0; i < listaCliente.Count(); i++)
            //{
            //    document.Add(new Paragraph("                     " + listaCliente[i].usuario + "                 " + listaCliente[i].nombre + "  " + listaCliente[i].apellido + "             " + listaCliente[i].codDoc + "        " + listaCliente[i].puntos));

            //}
            //document.Close();
            //String htmlText = html.ToString();
            //Document document = new Document();
            //string filePath = HostingEnvironment.MapPath("~/Content/Pdf/");
            //PdfWriter.GetInstance(document, new FileStream(filePath + "\\pdf-" + Filename + ".pdf", FileMode.Create));

            //document.Open();
            //iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
            //hw.Parse(new StringReader(htmlText));
            //document.Close();  
            //Response.Redirect("~/ReporteVentas.pdf");
            return RedirectToAction("ReporteCliente", "CuentaUsuario");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReporteCliente(ReporteClienteModel cliente)
        {
            if (ModelState.IsValid)
            {
                List<CuentaUsuario> listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.puntos > cliente.puntos && c.estado == true && c.codPerfil == 1).ToList();
                if (listacl != null) TempData["ListaPU"] = listacl;
                else TempData["ListaPU"] = null;
                return RedirectToAction("ReporteCliente", "CuentaUsuario");
            }
            TempData["ListaPU"] = null;
            return RedirectToAction("ReporteCliente", "CuentaUsuario");
        }

        public ActionResult Entrega(string usuario)
        {
            string usuario2 = usuario.Replace("°", "@");
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario2);
            TempData["EntregaCl"] = cuenta;
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public ActionResult Entrega2(string cliente)
        {
            string usuario2 = cliente.Replace("°", "@");
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario2);
            Session["EntregaCl"] = cuenta;
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public ActionResult PagoPendiente(string evId)
        {
            int m1;
            if (int.TryParse(evId, out m1) == false) return View();
            m1 = int.Parse(evId);
            Eventos ev = db.Eventos.Find(m1);
            Session["EventoSeleccionadoPago"] = m1;
            if (ev != null) Session["Pendiente"] = (double)ev.monto_adeudado - (double)ev.monto_transferir;
            return RedirectToAction("Pago", "Ventas");
        }

        public ActionResult PagoPendiente2(string evId)
        {
            int m1, np, nc;
            double p = 0, tc, tp;
            if (int.TryParse(evId, out m1) == false) return View();
            m1 = int.Parse(evId);
            Eventos ev = db.Eventos.Find(m1);
            Session["EventoSeleccionadoPago2"] = m1;
            if (ev != null)
            {
                List<Pago> lt = db.Pago.Where(c => c.codEvento == m1 && c.monto < 0).ToList();
                List<Funcion> le1 = db.Funcion.Where(c => c.codEvento == m1 && c.estado == "CANCELADO").ToList();
                List<Funcion> le2 = db.Funcion.Where(c => c.codEvento == m1 && c.estado == "POSTERGADO").ToList();
                nc = le1.Count;
                np = le2.Count;
                tc = nc * (double)ev.penalidadXcancelacion;
                tp = np * (double)ev.penalidadXpostergacion;
                for (int i = 0; i < lt.Count; i++)
                {
                    p += (double)lt[i].monto;
                }
                Session["Pendiente2"] = tc + tp + p;
            }
            return RedirectToAction("PagoOrganizador", "Ventas");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EntregaRegalo(RegaloListModel regalo)
        {
            CuentaUsuario cuenta2 = (CuentaUsuario)TempData["EntregaCl"];
            Regalo re = db.Regalo.Find(regalo.id);
            if (re.puntos < cuenta2.puntos)
            {
                db.Entry(cuenta2).State = EntityState.Modified;
                cuenta2.puntos = (int)cuenta2.puntos - (int)re.puntos;
                db.SaveChanges();
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public JsonResult EntregaRegalo2(string regalo, string cliente)
        {
            int idRe = int.Parse(regalo);
            string usuario2 = cliente.Replace("°", "@");
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario2);
            Regalo re = db.Regalo.Find(idRe);
            if (re.puntos <= cuenta.puntos)
            {
                db.Entry(cuenta).State = EntityState.Modified;
                cuenta.puntos = (int)cuenta.puntos - (int)re.puntos;
                db.SaveChanges();
                RegaloXCuenta rc = new RegaloXCuenta();
                rc.CuentaUsuario = cuenta;
                rc.fechaRecojo = DateTime.Now;
                rc.idRegalo = idRe;
                rc.Regalo = re;
                rc.usuario = usuario2;
                db.RegaloXCuenta.Add(rc);
                db.SaveChanges();
                db.Entry(cuenta).State = EntityState.Detached;
                return Json("Regalo Entregado", JsonRequestBehavior.AllowGet);
            }
            //CuentaUsuario cuenta2 = (CuentaUsuario)TempData["EntregaCl"];
            //Regalo re = db.Regalo.Find(regalo.id);
            //if (re.puntos < cuenta2.puntos)
            //{

            //}
            return Json("Error El cliente no tiene puntos suficientes para conseguir este regalo", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult RegistrarUsuarioVendedor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarUsuarioVendedor(RegistrarUsuarioVendedorModel model)
        {

            CuentaUsuario cu = new CuentaUsuario();

            cu.apellido = model.Apellidos;
            cu.correo = model.Correo;
            cu.codDoc = model.Dni;
            cu.tipoDoc = model.TipoDoc;
            cu.nombre = model.Nombres;


            db.CuentaUsuario.Add(cu);
            db.SaveChanges();


            TempData["tipo"] = "alert alert-success";
            TempData["message"] = "Datos Actualizados Exitosamente";
            return RedirectToAction("index2", "Home");
        }

    }
}
