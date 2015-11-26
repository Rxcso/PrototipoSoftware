using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    
    public class PoliticasController : Controller
    {
        inf245netsoft db = new inf245netsoft();
        [Authorize(Roles = "Administrador")]
        public ActionResult Politicas()
        {
            return View();
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
            else
            {
                if (dur != "")
                {
                    me1 = " 1.Error Número Decimal\n";
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
            else
            {
                if (mx != "")
                {
                    me2 = " 3.Error Número Decimal\n";
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
            else
            {
                if (mt != "")
                {
                    me3 = " 4.Error Número Decimal\n";
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
            else
            {
                if (ra != "")
                {
                    me4 = " 5.Error Número Decimal\n";
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
            else
            {
                if (mE != "")
                {
                    me5 = " 6.Error Número Decimal\n";
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
    }
}