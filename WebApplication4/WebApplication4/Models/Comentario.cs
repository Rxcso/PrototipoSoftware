using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    [Serializable]
    public class Comentario
    {
        public int codComentario { get; set; }
        public string contenido { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public int codEvento { get; set; }
        public string usuario { get; set; }

    }
}