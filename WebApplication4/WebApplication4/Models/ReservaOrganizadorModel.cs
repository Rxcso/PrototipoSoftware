using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ReservaOrganizadorModel
    {
        public int idEvento { get; set; }
        public string nameEvento { get; set; }
        public int[] idFuncion{get;set;}
        public string[] nameFuncion { get; set; }
        public int[] idZona { get; set;}
        public string[] nameZona { get; set; }
        public int[] idAsiento { get; set; }
        public string[] AsientoXFuncion{get;set;}
        public bool[] eliminar {get; set;}
    }
}