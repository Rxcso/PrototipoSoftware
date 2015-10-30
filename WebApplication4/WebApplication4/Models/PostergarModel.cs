using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class PostergarModel
    {

        public int idEvento { get; set; }

        public int idFuncion{ get; set; }

        public DateTime proximaFecha{ get; set; }

        public DateTime proximaHora{ get; set; }
    }
}