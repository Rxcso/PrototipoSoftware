using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    [Serializable]
    public class Coment
    {
        public string nombre { get; set; }
        public string contenido { get; set; }
        public DateTime? fecha { get; set; }
    }
}