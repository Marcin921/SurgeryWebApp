using PrzychodniaMvc.Models;
using PrzychodniaMvc.Models.BazaDanych;
using PrzychodniaMvc.Security;
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

            if (ModelState.IsValid)
            {
                using (PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7())
                {
                    //Trzeba dopisać sprawdzenie według numeru pesel czy taki pacjent  istnieje już w bazie danych
                    dc.Pacjent.Add(p);
                    dc.Uzytkownik.Add(p.Uzytkownik);
                    dc.SaveChanges();
                    
                    Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => t.Login == p.Uzytkownik.Login);
                    if (u.IdUzytkownika !=  0)
                    {
                        RolaUzytkownika rolaPacjenta = new RolaUzytkownika();
                        rolaPacjenta.IdUzytkownika = (int) p.IdUzytkownika;
                        rolaPacjenta.IdRoli = 4;
                        dc.RolaUzytkownika.Add(rolaPacjenta);
                    }
                    dc.SaveChanges();
                    
                    ModelState.Clear();
                    p = null; u = null;
                    RedirectToAction("Zaloguj");
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
            if (p.Uzytkownik.Haslo == null || p.Uzytkownik.Login == null)
            {
                return View(p);
            }
            else
            {
                PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
                Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => (t.Login == p.Uzytkownik.Login && 
                                                            t.Haslo == p.Uzytkownik.Haslo));
                if (u != null)
                {
                    FormsAuthentication.SetAuthCookie(u.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                }
                // If we got this far, something failed, redisplay form
                return View(u);
            }
        }
        public ActionResult Wyloguj()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}