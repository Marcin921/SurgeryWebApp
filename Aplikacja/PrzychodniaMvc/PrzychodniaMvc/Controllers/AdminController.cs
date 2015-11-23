using PrzychodniaMvc.Models.BazaDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PrzychodniaMvc.Controllers
{
    public class AdminController : Controller
    {
        public int IdLekarza { get; set; }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Zaloguj()
        {
            return View();
        }
        [HttpPost]
        // GET: Pacjent
        public ActionResult Zaloguj(Admin a)
        {
            if (a.Uzytkownik.Haslo == null || a.Uzytkownik.Login == null)
            {
                return View(a);
            }
            else
            {
                PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
                Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => (t.Login == a.Uzytkownik.Login &&
                                                            t.Haslo == a.Uzytkownik.Haslo));
                if (u != null)
                {
                    FormsAuthentication.SetAuthCookie(u.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                }
                // If we got this far, something failed, redisplay form
                return View(a);
            }
        }
        public ActionResult WyswietlUzytkownikow()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var uzytkownicy = dc.RolaUzytkownika;
            return View(uzytkownicy.ToList());
        }
        public ActionResult UtworzPacjenta()
        {
            return View();
        }
        public ActionResult UtworzRecepcjoniste()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Recepcjonista r = new Recepcjonista() { DataZatrudnienia = DateTime.Today };
            return View(r);
        }

        public ActionResult UtworzLekarza(SpecjalizacjaLekarza sl)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            if (ModelState.IsValid)
            {
                dc.SpecjalizacjaLekarza.Add(sl);
                dc.SaveChanges();

                Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => t.Login == sl.Lekarz.Uzytkownik.Login);
                if (u.IdUzytkownika != 0)
                {
                    RolaUzytkownika rolaLekarza = new RolaUzytkownika();
                    rolaLekarza.IdUzytkownika = (int)sl.Lekarz.IdUzytkownika;
                    rolaLekarza.IdRoli = 3;
                    dc.RolaUzytkownika.Add(rolaLekarza);
                }
                dc.SaveChanges();

            }

            if (sl.Specjalizacja == null)
            {
                sl.Specjalizacja = new Specjalizacja();
            }

            if (sl.Lekarz == null)
            {
                sl.Lekarz = new Lekarz();
            }

            if (sl.Specjalizacja.ListaSpecjalizacji == null)
            {
                sl.Specjalizacja.ListaSpecjalizacji = dc.Specjalizacja.OrderBy(s => s.NazwaSpecjalizacji).Select(s =>
                      new SelectListItem
                      {
                          Selected = true,
                          Text = s.NazwaSpecjalizacji,
                          Value = s.IdSpecjalizacji.ToString()
                      });
            }

            if (dc.Lekarz.Count(ll => ll.IdLekarza == sl.Lekarz.IdLekarza) == 0)
            {
                sl.Lekarz.DataZatrudnienia = DateTime.Today;
            }
            
                return View(sl);
        }

        public ActionResult EdytujDniIGodzinyPracy(Nullable<int> id)
        {
            if (id != null)
            {
                IdLekarza = (int)id;
            }

            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            return View(dc.DzienGodzinaPracyLekarza.Where(t => t.IdLekarza == id).ToList());
        }

        public ActionResult UtworzDzienIGodzinePracy()
        {
            
            return View();
        }


    }
}