using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ZonaModel
    {
        //public int idEvento { get; set; }    
        //public int aforo {get; set; }
        //public boolean tieneAsientos{get; set;}
        public int idZona {get; set;}

        public int filas{get;set;}
        public int columnas { get; set; }

        public List<int> posFila { get; set; }
        public List<int> posCol { get; set; }
    }

}