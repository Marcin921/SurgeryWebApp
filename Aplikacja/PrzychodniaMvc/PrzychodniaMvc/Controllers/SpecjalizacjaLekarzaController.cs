using PrzychodniaMvc.Models.BazaDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Controllers
{
    public class SpecjalizacjaLekarzaController : Controller
    {
        // GET: SpecjalizacjaLekarza
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UtworzLekarzaISpecjalizacje(SpecjalizacjaLekarza sl)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            if (sl.Lekarz == null ||  dc.Lekarz.Count(ll => ll.IdLekarza == sl.Lekarz.IdLekarza) == 0)
            {
                SpecjalizacjaLekarza sll = new SpecjalizacjaLekarza() { Lekarz = new Lekarz() { DataZatrudnienia = DateTime.Today } };
                return View();
            }
            return View();
        }
    }
}