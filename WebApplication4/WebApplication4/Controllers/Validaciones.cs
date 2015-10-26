using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{

    public class VerificacionBTV
    {
        public VerificacionBTV()
        {
            esCorrecto = true;
        }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public bool esCorrecto { get; set; }
        public string razon { get; set; }

    }
    public class DateValidationMethods
    {
        public static List<VerificacionBTV> GetVericationFormat(BloqueTiempoListModel model)
        {
            List<VerificacionBTV> listaVer = new List<VerificacionBTV>();
            for (int i = 0; i < model.ListaBTM.Count; i++)
            {
                DateTime inicio = DateTime.Parse(model.ListaBTM[i].fechaInicio);
                DateTime fin = DateTime.Parse(model.ListaBTM[i].fechaFin);
                VerificacionBTV itemList = new VerificacionBTV();
                itemList.fechaInicio = inicio;
                itemList.fechaFin = fin;
                listaVer.Add(itemList);
            }
            return listaVer;
        }
        public static bool VerifyOverlapDates(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return (DateTime.Compare(startA, endB) < 0) && (DateTime.Compare(endA, startB) > 0);
        }
    }

    public class Validaciones
    {

        public static List<VerificacionBTV> ValidarBloquesDeTiempoDeVenta(BloqueTiempoListModel model)
        {
            //bloquetiempolistmodel tiene los datos en string, hay que crearlo ahora con date
            List<VerificacionBTV> listaVer = DateValidationMethods.GetVericationFormat(model);
            for (int i = 0; i < listaVer.Count-1; i++)
            {
                for (int j = i + 1; j < listaVer.Count; j++)
                {
                    if (DateValidationMethods.VerifyOverlapDates(listaVer[i].fechaInicio, listaVer[i].fechaFin, listaVer[j].fechaInicio, listaVer[j].fechaFin))
                    {
                        listaVer[i].esCorrecto = false;
                        listaVer[i].razon = "Cruce con bloque de tiempo de venta #" + j;
                    }
                }
            }
            return listaVer;
        }
    }
}