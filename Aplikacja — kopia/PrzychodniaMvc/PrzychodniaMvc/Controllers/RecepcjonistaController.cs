using PrzychodniaMvc.Models.BazaDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PrzychodniaMvc.Controllers
{
    public class RecepcjonistaController : Controller
    {
        // GET: Recepcjonista
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Zaloguj()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Zaloguj(Recepcjonista r)
        {
            if (r.Uzytkownik.Haslo == null || r.Uzytkownik.Login == null)
            {
                return View(r);
            }
            else
            {
                PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
                Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => (t.Login == r.Uzytkownik.Login &&
                                                            t.Haslo == r.Uzytkownik.Haslo));
                if (u != null)
                {
                    FormsAuthentication.SetAuthCookie(u.Login, true);
                    return RedirectToAction("UtworzKalendarz", "KalendarzDlaRecepcjonisty");
                }
                else
                {
                }
                // If we got this far, something failed, redisplay form
                return View(r);
            }
        }
        public ActionResult Wyloguj()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}