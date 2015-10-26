using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class DateValidationMethods
    {
        public static bool VerifyOverlapDates(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return (DateTime.Compare(startA, endB) < 0) && (DateTime.Compare(endA, startB) > 0);
        }
    }

    public class Validaciones
    {

        public static List<BloqueDeTiempoModel> ValidarBloquesDeTiempoDeVenta(BloqueTiempoListModel model)
        {
            //bloquetiempolistmodel tiene los datos en string, hay que crearlo ahora con date
            //List<VerificacionBTV> listaVer = DateValidationMethods.GetVericationFormat(model);
            List<BloqueDeTiempoModel> listaVer = model.ListaBTM;
            for (int i = 0; i < listaVer.Count - 1; i++)
            {
                for (int j = i + 1; j < listaVer.Count; j++)
                {
                    if (DateValidationMethods.VerifyOverlapDates(listaVer[i].fechaInicio, listaVer[i].fechaFin, listaVer[j].fechaInicio, listaVer[j].fechaFin))
                    {
                        listaVer[i].esCorrecto = false;
                        listaVer[i].razon = "Cruce con bloque de tiempo de venta #" + (j+1);
                    }
                }
            }
            return listaVer;
        }
    }
}