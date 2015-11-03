using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class PaqueteEntradas
    {
        public int idEvento { get; set; }
        public int idFuncion { get; set; }
        public int idZona { get; set; }

        public List<int> filas { get; set; }
        public List<int> columnas { get; set; }

        public PaqueteEntradas(int id)
        {
            idEvento = id;
            idFuncion=0;
            idZona=0;
        }
    }
}