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
    public class CuentaUsuarioController : Controller
    {
        private inf245netsoft db = new inf245netsoft();

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
        public ActionResult Create([Bind(Include="usuario,tipoUsuario,correo,contrasena,estado,tipoDoc,codDoc,nombre,apellido,direccion,telefono,telMovil,sexo,fechaNac,puntos,codPerfil")] CuentaUsuario cuentausuario)
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
        public ActionResult Edit([Bind(Include="usuario,tipoUsuario,correo,contrasena,estado,tipoDoc,codDoc,nombre,apellido,direccion,telefono,telMovil,sexo,fechaNac,puntos,codPerfil")] CuentaUsuario cuentausuario)
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
                List<CuentaUsuario> listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == cliente.tipoDoc && c.codDoc==cliente.codDoc && c.estado == true && c.codPerfil == 1).ToList();
                if (listacl != null) TempData["ListaCL"] = listacl;
                else TempData["ListaCL"] = null;
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            TempData["ListaCL"] = null;
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public ActionResult Search2(string usuario,string tipo)
        {
            if (tipo == "")
            {
                Session["ListaCL"] = null;
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            int ti = int.Parse(tipo);
            if (ti == 0)
            {
                Session["ListaCL"] = null;
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            if (usuario == "" || usuario == null)
            {
                Session["ListaCL"] = null;
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            List<CuentaUsuario> listacl = db.CuentaUsuario.AsNoTracking().Where(c => c.tipoDoc == ti && c.codDoc == usuario && c.estado == true && c.codPerfil == 1 && c.puntos>0).ToList();
            if (listacl != null) Session["ListaCL"] = listacl;
            else Session["ListaCL"] = null;
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
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
            return View();
        }

        public ActionResult MisReservas()
        {
            return View();
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
            List<WebApplication4.Models.CuentaUsuario> listaCliente = (List<WebApplication4.Models.CuentaUsuario>)TempData["ListaPU"];
            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream("F://ReporteVentas.pdf", FileMode.OpenOrCreate));
            document.Open();
            DateTime d1 = DateTime.Now;
            document.Add(new Paragraph(""));
            document.Add(new Paragraph("                                                            Reporte de Clientes"));
            document.Add(new Paragraph("            Fecha:               " + d1.Date + "                     "));
            document.Add(new Paragraph("                     Usuario        Nombre y Apellido             Codigo Documento       Puntos"));
            for (int i = 0; i < listaCliente.Count(); i++)
            {
                document.Add(new Paragraph("                     " + listaCliente[i].usuario + "                 " + listaCliente[i].nombre + "  " + listaCliente[i].apellido + "             " + listaCliente[i].codDoc + "        " + listaCliente[i].puntos));

            }
            document.Close();
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EntregaRegalo(RegaloListModel regalo)
        {
            CuentaUsuario cuenta2 = (CuentaUsuario)TempData["EntregaCl"];
            Regalo re = db.Regalo.Find(regalo.id);
            if (re.puntos < cuenta2.puntos)
            {
                db.Entry(cuenta2).State = EntityState.Modified;
                cuenta2.puntos = cuenta2.puntos - re.puntos;
                db.SaveChanges();
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

        public ActionResult EntregaRegalo2(string regalo, string cliente)
        {
            int idRe = int.Parse(regalo);
            string usuario2 = cliente.Replace("°", "@");
            CuentaUsuario cuenta = db.CuentaUsuario.Find(usuario2);
            Regalo re = db.Regalo.Find(idRe);
            if (re.puntos <= cuenta.puntos)
            {
                db.Entry(cuenta).State = EntityState.Modified;
                cuenta.puntos = cuenta.puntos - re.puntos;
                //db.SaveChanges();
                RegaloXCuenta rc = new RegaloXCuenta();
                rc.CuentaUsuario = cuenta;
                rc.fechaRecojo = DateTime.Now;
                rc.idRegalo = idRe;
                rc.Regalo = re;
                rc.usuario = usuario2;
                db.RegaloXCuenta.Add(rc);
                db.SaveChanges();
                return RedirectToAction("BuscaCliente", "CuentaUsuario");
            }
            //CuentaUsuario cuenta2 = (CuentaUsuario)TempData["EntregaCl"];
            //Regalo re = db.Regalo.Find(regalo.id);
            //if (re.puntos < cuenta2.puntos)
            //{
            
            //}
            return RedirectToAction("BuscaCliente", "CuentaUsuario");
        }

    }
}
