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
            return (DateTime.Compare(startA.Date, endB.Date) < 0) && (DateTime.Compare(endA.Date, startB.Date) > 0);
        }
        public static int VerifyDifferentHours(DateTime hourA, DateTime hourB)
        {
            return TimeSpan.Compare(hourA.TimeOfDay, hourB.TimeOfDay);
        }
        public static List<BloqueDeTiempoModel> QuitaDuplicados(List<BloqueDeTiempoModel> bloques)
        {
            return bloques.GroupBy(c => c.fechaInicio).Select(s => s.First()).ToList();
        }
        public static string GetMonthName(int id){
            switch (id){
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12: 
                    return "Diciembre";
                default:
                    return "Mes";
            }
        }
    }

    public class Validaciones
    {
        
        public static List<BloqueDeTiempoModel> ValidarBloquesDeTiempoDeVenta(BloqueTiempoListModel model)
        {
            //bloquetiempolistmodel tiene los datos en string, hay que crearlo ahora con date
            List<BloqueDeTiempoModel> listaVer = model.ListaBTM;
            //quito bloques duplicados
            listaVer = DateValidationMethods.QuitaDuplicados(listaVer);
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

        public static List<FuncionesModel> ValidarFunciones(FuncionesListModel model)
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

        public static bool VerificaEventoDG(DatosGeneralesModel model)
        {
            return (model.Local == 0) ^ (model.Direccion == null);
        }
    }


}