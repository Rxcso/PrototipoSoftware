using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class EmpleadoController : Controller
    {

        private inf245netsoft db = new inf245netsoft();

        // GET: Empleado
        public ActionResult Index()
        {
            return View();
        }
        

        
    }
}