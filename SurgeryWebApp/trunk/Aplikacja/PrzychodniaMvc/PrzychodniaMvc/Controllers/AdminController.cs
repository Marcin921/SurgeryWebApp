using PrzychodniaMvc.Models.BazaDanych;
using PrzychodniaMvc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PrzychodniaMvc.Controllers
{
    [AuthorizeRoles("Administrator")]
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

        public ActionResult UtworzRecepcjoniste(Recepcjonista r)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            if(ModelState.IsValid)
            {
                dc.Recepcjonista.Add(r);
                dc.SaveChanges();

                Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => t.Login == r.Uzytkownik.Login);
                if (u.IdUzytkownika != 0)
                {
                    RolaUzytkownika rolaRecepcjonisty = new RolaUzytkownika();
                    rolaRecepcjonisty.IdUzytkownika = (int)r.IdUzytkownika;
                    rolaRecepcjonisty.IdRoli = 2;
                    dc.RolaUzytkownika.Add(rolaRecepcjonisty);
                }
                dc.SaveChanges();
            }


            if (dc.Recepcjonista.Count(rr => rr.IdRecepjonisty == r.IdRecepjonisty) == 0)
            {
                r.DataZatrudnienia = DateTime.Today;
            }
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
            if (id != null && id != 0)
            {
                IdLekarza = (int)id;
            }


            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            return View(dc.DzienGodzinaPracyLekarza.Where(t => t.IdLekarza == id).ToList());
        }

        public ActionResult UtworzDzienIGodzinePracy(DzienGodzinaPracyLekarza dg,int id)
        {
            if(ModelState.IsValid && id != 0)
            {
                PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
                dg.IdLekarza = id;
                dc.DzienGodzinaPracyLekarza.Add(dg);
                dc.SaveChanges();
                return RedirectToAction("EdytujDniIGodzinyPracy", new { id = id } );
            }

            var dniTygodnia = new List<SelectListItem>();
            dniTygodnia.Add(new SelectListItem { Selected = true, Text = "Poniedziałek", Value = "1" });
            dniTygodnia.Add(new SelectListItem { Text = "Wtorek", Value = "2" });
            dniTygodnia.Add(new SelectListItem { Text = "Środa", Value = "3" });
            dniTygodnia.Add(new SelectListItem { Text = "Czwartek", Value = "4" });
            dniTygodnia.Add(new SelectListItem { Text = "Piątek", Value = "5" });

            dg.ListaDniTygodnia = dniTygodnia;

            return View(dg);
        }

        public ActionResult UsunUzytkownika(Nullable<int> id)
        {
            if (id != 0 && id != null)
            {
                PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
                dc.Uzytkownik.Remove(dc.Uzytkownik.FirstOrDefault(u => u.IdUzytkownika == (int)id));
                dc.SaveChanges();
            }
            return RedirectToAction("WyswietlUzytkownikow");
        }

        public ActionResult EdytujUzytkownika(Nullable<int> id)
        {
            if (id != 0 && id != null)
            {
                PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
                RolaUzytkownika r = dc.RolaUzytkownika.FirstOrDefault(rr => rr.IdUzytkownika == id);
                if(r.IdRoli == 1)
                {
                    return RedirectToAction("EdytujAdmina", new { id = id });
                }
                else if(r.IdRoli == 2)
                {
                    return RedirectToAction("EdytujRecepcjoniste", new { id = id });
                }
                else if (r.IdRoli == 3)
                {
                    return RedirectToAction("EdytujLekarza", new { id = id });
                }
            }
            return RedirectToAction("WyswietlUzytkownikow");
        }

        public ActionResult EdytujAdmina(Admin admin, Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Admin a = dc.Admin.FirstOrDefault(aa => aa.IdUzytkownika == id);
            return View(a);
        }

        public ActionResult EdytujRecepcjoniste(Recepcjonista rec, Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Recepcjonista r = dc.Recepcjonista.FirstOrDefault(rr => rr.IdUzytkownika == id);
            return View(r);
        }

        public ActionResult EdytujLekarza(SpecjalizacjaLekarza sll,  Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            if (ModelState.IsValid)
            {
                Lekarz l = dc.Lekarz.Single(ll => ll.Uzytkownik.IdUzytkownika == id);
                l.Imie = sll.Lekarz.Imie;
                l.Nazwisko = sll.Lekarz.Nazwisko;
                l.NumerTelefonu = sll.Lekarz.NumerTelefonu;
                l.Uzytkownik.Haslo = sll.Lekarz.Uzytkownik.Haslo;
                l.Uzytkownik.Login = sll.Lekarz.Uzytkownik.Login;
                SpecjalizacjaLekarza sl_ = dc.SpecjalizacjaLekarza.Single(slll => slll.IdLekarza == l.IdLekarza);
                sl_.IdSpecjalizacji = sll.IdSpecjalizacji;
                dc.SaveChanges();
            }
           
                SpecjalizacjaLekarza sl = dc.SpecjalizacjaLekarza.FirstOrDefault(ll => ll.Lekarz.Uzytkownik.IdUzytkownika == id);
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
                return View(sl);
        }


    }
}