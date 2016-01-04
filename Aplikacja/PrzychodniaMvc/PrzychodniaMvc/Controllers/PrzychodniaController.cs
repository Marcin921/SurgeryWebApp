using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Controllers
{
    public class PrzychodniaController : Controller
    {
        // GET: Przychodnia
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Kontakt()
        {
            return View();
        }
        public ActionResult Onas()
        {
            return View();
        }
    }
}