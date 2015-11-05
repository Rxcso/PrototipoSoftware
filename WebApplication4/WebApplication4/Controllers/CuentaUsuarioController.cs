using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class CuentaUsuarioController : Controller
    {
        private inf245netsoft db = new inf245netsoft();

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
            //AspNetUsers user = db.AspNetUsers.Where(c => c.Email == correo).First();
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
            }
            else
            {
                //Error

            }
            return RedirectToAction("MiCuenta");
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
            return  cuentausuario && aspNetuser ;
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

        // GET: /CuentaUsuario/
        public ActionResult Index()
        {
            return View(db.CuentaUsuario.ToList());
        }

        public ActionResult BuscaCliente()
        {
            return View();
        }

        // GET: /CuentaUsuario/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            if (cuentausuario == null)
            {
                return HttpNotFound();
            }
            return View(cuentausuario);
        }

        // GET: /CuentaUsuario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /CuentaUsuario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "usuario,tipoUsuario,correo,contrasena,estado,tipoDoc,codDoc,nombre,apellido,direccion,telefono,telMovil,sexo,fechaNac,puntos,codPerfil")] CuentaUsuario cuentausuario)
        {
            if (ModelState.IsValid)
            {
                db.CuentaUsuario.Add(cuentausuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cuentausuario);
        }

        // GET: /CuentaUsuario/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            if (cuentausuario == null)
            {
                return HttpNotFound();
            }
            return View(cuentausuario);
        }

        // POST: /CuentaUsuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "usuario,tipoUsuario,correo,contrasena,estado,tipoDoc,codDoc,nombre,apellido,direccion,telefono,telMovil,sexo,fechaNac,puntos,codPerfil")] CuentaUsuario cuentausuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentausuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuentausuario);
        }

        // GET: /CuentaUsuario/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            if (cuentausuario == null)
            {
                return HttpNotFound();
            }
            return View(cuentausuario);
        }

        // POST: /CuentaUsuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(id);
            db.CuentaUsuario.Remove(cuentausuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
            if (ti == 0) listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc == usuario && c.estado == true && c.codPerfil == 1 && c.puntos > 0).ToList();
            else listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == ti && c.codDoc == usuario && c.estado == true && c.codPerfil == 1 && c.puntos > 0).ToList();
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
            List<CuentaUsuario> listacl;
            if (ti == 0) listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.codDoc.Contains(usuario) && c.estado == true && c.codPerfil == 1).ToList();
            else listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == ti && c.codDoc.Contains(usuario) && c.estado == true && c.codPerfil == 1).ToList();
            if (listacl == null) return RedirectToAction("BuscaReserva", "CuentaUsuario");

            List<Ventas> listareservas = new List<Ventas>();
            for (int i = 0; i < listacl.Count; i++)
            {
                string us = listacl[i].usuario;
                List<Ventas> lv = db.Ventas.Where(c => c.cliente == us && c.Estado == "Reservado").ToList();
                for (int j = 0; j < lv.Count; j++)
                {
                    listareservas.Add(lv[j]);
                }
            }
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

        public ActionResult DeleteReserva(int codE, int codF)
        {
            Ventas v = db.Ventas.Find(codE);
            db.Entry(v).State = EntityState.Modified;
            v.Estado = "Anulado";
            db.SaveChanges();
            db.Entry(v).State = EntityState.Detached;
            //Session["listaReservaClientes"]=db.
            return RedirectToAction("MisReservas", "CuentaUsuario");
        }

        public JsonResult RegistraPoliticas(string dur, string mx, string mt, string ra, string mE, string hr)
        {
            int m1, m2, m3, m4, m5, m6;
            string me1 = "Error", me2 = " Error", me3 = " Error", me4 = " Error", me5 = " Error", me6 = " Error";
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
                    me1 = "Completado";
                }
                else
                {
                    me1 = " Error Negativo";
                }
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
                    me2 = " Completado";
                }
                else
                {
                    me2 = " Error Negativo";
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
                    me3 = " Completado";
                }
                else
                {
                    me3 = " Error Negativo";
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
                    me4 = " Completado";
                }
                else
                {
                    me4 = " Error Negativo";
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
                    me5 = " Completado";
                }
                else
                {
                    me5 = " Error Negativo";
                }
            }
            if (int.TryParse(hr, out m6) == true)
            {
                int val6 = int.Parse(hr);
                if (val6 > 0)
                {
                    int t = 6;
                    Politicas p = db.Politicas.Find(t);
                    db.Entry(p).State = EntityState.Modified;
                    p.valor = val6;
                    db.SaveChanges();
                    db.Entry(p).State = EntityState.Detached;
                    me6 = " Completado";
                }
                else
                {
                    me6 = " Error Negativo";
                }
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
            int limite = (int)db.Politicas.Find(idPol).valor;
            if (dt1 <= DateTime.Now) return Json("la fecha debe ser superior de hoy", JsonRequestBehavior.AllowGet);
            if (dt1 > dt2) return Json("Fecha inicio debe ser menor que fecha fin", JsonRequestBehavior.AllowGet);
            if (nd > limite) return Json("No puedo asignar a la vez mas de "+limite+" turnos de manera seguida", JsonRequestBehavior.AllowGet);
            //int cruce = 0;            
            for (int j = 0; j < nd; j++)
            {
                List<Turno> ltur = db.Turno.Where(c => c.codPuntoVenta == idp && c.codTurnoSis == idt && di == c.fecha).ToList();
                if (ltur.Count != 0)
                {
                    //Session["nError"] = 1;
                    //TempData["ErrorAsignacion"] = "Cruce con el usuario " + ltur.First().usuario + " para el dia " + di;
                    //return RedirectToAction("Asignacion", "CuentaUsuario");
                    string mensaje = "Cruce con el usuario " + ltur.First().usuario + " para el dia " + di.ToString("dd/MM/yyyy");

                    return Json(mensaje, JsonRequestBehavior.AllowGet);
                }
                di = di.AddDays(1);
            }
            //int cruce1 = 0;
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

        public ActionResult Carrito()
        {
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
            int m1;
            if (int.TryParse(evId, out m1) == false) return View();
            m1 = int.Parse(evId);
            Eventos ev = db.Eventos.Find(m1);
            Session["EventoSeleccionadoPago2"] = m1;
            if (ev != null) Session["Pendiente2"] = (double)ev.penalidadXcancelacion;
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
                //db.SaveChanges();
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
        public ActionResult RegistrarUsuarioVendedor(){
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarUsuarioVendedor(RegistrarUsuarioVendedorModel model){

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
