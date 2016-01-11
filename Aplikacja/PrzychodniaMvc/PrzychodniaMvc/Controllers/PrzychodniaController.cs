using PrzychodniaMvc.Models.BazaDanych;
using PrzychodniaMvc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Controllers
{
    public class PrzychodniaController : Controller
    {
        [AuthorizeRoles("Admin")]
        public ActionResult AdminOnly()
        {
            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }
        // GET: Przychodnia
        public ActionResult Index()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            
                var s = dc.Specjalizacja.ToList().
                Where(ss => dc.SpecjalizacjaLekarza.FirstOrDefault(sl => sl.IdSpecjalizacji == ss.IdSpecjalizacji) != null);

                return View(s);
        }
        public ActionResult Kontakt()
        {
            return View();
        }
        public ActionResult Onas()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();

            var sl = dc.SpecjalizacjaLekarza.OrderBy(sll => sll.Specjalizacja.NazwaSpecjalizacji).ToList();
            return View(sl);
        }
    }
}