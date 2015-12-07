using PrzychodniaMvc.Models.BazaDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Controllers
{
    public class LekarzController : Controller
    {
        // GET: Lekarz
        public ActionResult Index()
        {
            
            return View();
        }

    }
}