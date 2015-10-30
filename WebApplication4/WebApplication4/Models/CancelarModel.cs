using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class CancelarModel
    {

        public int idEvento;

        public string nombreEvento;

        public string organizador;

        public List<int> listIdFuncion;

        public List<Boolean> seCancela;

        public DateTime fechaRecojo;

        public int diasRecojo;

        public string motivo;

    }
}