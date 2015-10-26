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

    }
    public class GetterMethods{
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
    }
  
    public class Validaciones
    {
        
        public static bool ValidarBloquesDeTiempoDeVenta(BloqueTiempoListModel model)
        {
            //bloquetiempolistmodel tiene los datos en string, hay que crearlo ahora con date
            List<VerificacionBTV> listaVer = GetterMethods.GetVericationFormat(model);
            
            return false;
        }
    }
}