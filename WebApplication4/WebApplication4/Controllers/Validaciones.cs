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
        public static int VerifyDifferentHours(DateTime hourA, DateTime hourB)
        {
            return TimeSpan.Compare(hourA.TimeOfDay, hourB.TimeOfDay);
        }
    }

    public class Validaciones
    {
        public static List<BloqueDeTiempoModel> ValidarBloquesDeTiempoDeVenta(BloqueTiempoListModel model)
        {
            //bloquetiempolistmodel tiene los datos en string, hay que crearlo ahora con date
            List<BloqueDeTiempoModel> listaVer = model.ListaBTM;
            bool esCorrecto = true;
            for (int i = 0; i < listaVer.Count - 1; i++)
            {
                for (int j = i + 1; j < listaVer.Count; j++)
                {
                    if (DateValidationMethods.VerifyOverlapDates(listaVer[i].fechaInicio, listaVer[i].fechaFin, listaVer[j].fechaInicio, listaVer[j].fechaFin))
                    {
                        esCorrecto = false;
                        listaVer[i].razon = "Cruce con bloque de tiempo de venta #" + (j + 1);
                    }
                }
            }
            model.esCorrecto = esCorrecto;
            return listaVer;
        }

        internal static List<FuncionesModel> ValidarFunciones(FuncionesListModel model)
        {
            List<FuncionesModel> listaVer = model.ListaFunciones;
            bool esCorrecto = true;
            var queryFuncion = from obj in listaVer
                               group obj by obj.fechaFuncion into newGFunciones
                               orderby newGFunciones.Key
                               select newGFunciones;
            foreach (var fGroup in queryFuncion)
            {
                List<DateTime> listComp = new List<DateTime>();
                foreach (var dFuncion in fGroup)
                {
                    listComp.Add(dFuncion.horaInicio);
                }
                for (int i = 0; i < listComp.Count-1; i++)
                {
                    for (int j = i + 1; j < listComp.Count; j++)
                    {
                        if (DateValidationMethods.VerifyDifferentHours(listComp[i], listComp[j]) == 0)
                        {
                            esCorrecto = false;
                        }
                    }
                }
            }
            model.esCorrecto = esCorrecto;
            return listaVer;
        }
    }


}