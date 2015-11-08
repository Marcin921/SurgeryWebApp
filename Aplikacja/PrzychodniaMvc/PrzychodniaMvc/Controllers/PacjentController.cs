using PrzychodniaMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PrzychodniaMvc.Controllers
{
    public class PacjentController : Controller
    {
        // GET: Pacjent
        public ActionResult Rejestruj(Pacjent p)
        {
            if (!ModelState.IsValid)
            {
                using (PrzychodniaBDEntities dc = new PrzychodniaBDEntities())
                {
                    //Trzeba dopisać sprawdzenie według numeru pesel czy taki pacjent  istnieje już w bazie danych
                    dc.Pacjent.Add(p);
                    dc.SaveChanges();
                    ModelState.Clear();
                    p = null;
                    ViewBag.Message = "Twoje konto zostało założone. Możesz się teraz zalogować.";
                }
            }
            return View(p);
        }

        public ActionResult Zaloguj()
        {
            return View();
        }
        [HttpPost]
        // GET: Pacjent
        public ActionResult Zaloguj(Pacjent p)
        {
            if (p.Hasło == null || p.Login == null)
            {
                return View(p);
            }
            else
            {
                PrzychodniaBDEntities dc = new PrzychodniaBDEntities();
                Pacjent pacjent = dc.Pacjent.FirstOrDefault(t => (t.Login == p.Login && t.Hasło == p.Hasło));
                if (p != null)
                {
                    FormsAuthentication.SetAuthCookie(p.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                { }
                // If we got this far, something failed, redisplay form
                return View(p);
            }
        }

        public ActionResult Wyloguj()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}