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
                    return RedirectToAction("WyswietlUzytkownikow", "Admin");
                }
                else
                {
                }
                // If we got this far, something failed, redisplay form
                return View(a);
            }
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult WyswietlUzytkownikow()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var uzytkownicy = dc.RolaUzytkownika;
            return View(uzytkownicy.ToList());
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult UtworzPacjenta(Pacjent p)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            if (ModelState.IsValid)
            {
                dc.Pacjent.Add(p);
                dc.SaveChanges();

                Uzytkownik u = dc.Uzytkownik.FirstOrDefault(t => t.Login == p.Uzytkownik.Login);
                if (u.IdUzytkownika != 0)
                {
                    RolaUzytkownika rolaPacjenta = new RolaUzytkownika();
                    rolaPacjenta.IdUzytkownika = (int)p.IdUzytkownika;
                    rolaPacjenta.IdRoli = 2;
                    dc.RolaUzytkownika.Add(rolaPacjenta);
                }
                dc.SaveChanges();
            }
            return View();
        }
        [AuthorizeRoles("Administrator")]
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
        [AuthorizeRoles("Administrator")]
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
                sl.Lekarz.DataRozpWizyt = DateTime.Today;
                sl.Lekarz.DataZakWizyt = new DateTime(DateTime.Now.Year, 12, 31);
            }
            
                return View(sl);
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult EdytujDniIGodzinyPracy(IList<DzienGodzinaPracyLekarza> dg, int? id)
        {
            if (id != null && id != 0)
            {
                IdLekarza = (int)id;
            }


            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            return View(dc.DzienGodzinaPracyLekarza.Where(t => t.IdLekarza == id).ToList());
        }
        [AuthorizeRoles("Administrator")]
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

            dg.listaDniTygodnia = dniTygodnia;

            return View(dg);
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult GenerujTerminy(Lekarz l, Nullable<int> id, string typ)
        {
            DateTime? d1 = l.DataRozpWizyt;
            DateTime? d2 = l.DataZakWizyt;
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            if (id != 0 && id != null && l.IdLekarza == 0)
            {
                l = dc.Lekarz.FirstOrDefault(ll => ll.IdLekarza == id);
            }
            if(typ != null && typ.Equals("Generuj Terminy"))
            {
                l.DataRozpWizyt = d1;
                l.DataZakWizyt = d2;
                l.Uzytkownik = l.Uzytkownik;
                try
                {
                    dc.SaveChanges();
                }
                catch(Exception e)
                    {

                }
                List<int> dni_przyjec = dc.DzienGodzinaPracyLekarza.Where(dgg => dgg.IdLekarza == id).Select(a =>  a.DzienTygodnia).ToList();
                List<DzienGodzinaPracyLekarza> dg = dc.DzienGodzinaPracyLekarza.Where(dgg => dgg.IdLekarza == id).ToList();
                Rejestracja r = new Rejestracja();
                r.IdLekarza = (int) id;
                r.CzyZajeta = "N";
                while (d1.Value <= d2.Value)
                {
                   if(dni_przyjec.Contains((int)d1.Value.DayOfWeek))
                    {
                        DzienGodzinaPracyLekarza dgdg = dg.FirstOrDefault(dd => dd.DzienTygodnia == (int)d1.Value.DayOfWeek);
                        string g1 = dgdg.GodzinaRozp;
                        string g2 = dgdg.GodzinaZak;
                        string czas = dgdg.CzasJednejWizyty;
                        DateTime t1= DateTime.Parse(g1, System.Globalization.CultureInfo.CurrentCulture);
                        DateTime t2 = DateTime.Parse(g2, System.Globalization.CultureInfo.CurrentCulture);
                        t1 = new DateTime(d1.Value.Year, d1.Value.Month, d1.Value.Day, t1.Hour, t1.Minute, t1.Second);
                        t2 = new DateTime(d1.Value.Year, d1.Value.Month, d1.Value.Day, t2.Hour, t2.Minute, t1.Second);

                        while(t1<t2)
                        {
                            r.DataRozp = t1;
                            r.DataZak = t1.AddMinutes(Convert.ToDouble(czas));

                            dc.Rejestracja.Add(r);
                            try
                            {
                                dc.SaveChanges();
                            }
                            catch (Exception exception)
                            {

                            }
                            
                            t1 = t1.AddMinutes(Convert.ToDouble(czas));
                            
                        }
                    }
                    d1 = d1.Value.AddDays(1);
                }       
            }

            return View(l);
        }
        [AuthorizeRoles("Administrator")]
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
        [AuthorizeRoles("Administrator")]
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
                else if (r.IdRoli == 4)
                {
                    return RedirectToAction("EdytujPacjenta", new { id = id });
                }
            }
            return RedirectToAction("WyswietlUzytkownikow");
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult EdytujAdmina(Admin admin, Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Admin a = dc.Admin.FirstOrDefault(aa => aa.IdUzytkownika == id);
            if(ModelState.IsValid)
            {
                a.Imie = admin.Imie;
                a.Nazwisko = admin.Nazwisko;
                a.Uzytkownik.Haslo = admin.Uzytkownik.Haslo;
                a.Uzytkownik.Login = admin.Uzytkownik.Login;
                dc.SaveChanges();
            }
         
            return View(a);
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult EdytujRecepcjoniste(Recepcjonista rec, Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            
            Recepcjonista r = dc.Recepcjonista.FirstOrDefault(rr => rr.IdUzytkownika == id);
            if (ModelState.IsValid)
            {
                r.Imie = rec.Imie;
                r.Nazwisko = rec.Nazwisko;
                r.NumerTelefonu = rec.NumerTelefonu;
                r.Uzytkownik.Haslo = rec.Uzytkownik.Haslo;
                r.Uzytkownik.Login = rec.Uzytkownik.Login;
                dc.SaveChanges();
            }
            return View(r);
        }
        [AuthorizeRoles("Administrator")]
        public ActionResult EdytujPacjenta(Pacjent p, Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Pacjent pp = dc.Pacjent.FirstOrDefault(ppp => ppp.IdUzytkownika == id);
            if(ModelState.IsValid)
            {
                pp.Imie = p.Imie;
                pp.Nazwisko = p.Nazwisko;
                pp.KodPocztowy = p.KodPocztowy;
                pp.Miasto = p.Miasto;
                pp.NumerTelefonu = p.NumerTelefonu;
                pp.Pesel = p.Pesel;
                pp.Zatwierdzono = p.Zatwierdzono;
                pp.Uzytkownik.Haslo = p.Uzytkownik.Haslo;
                pp.Uzytkownik.Login = p.Uzytkownik.Login;
                dc.SaveChanges();
            }
            return View(pp);
        }
        [AuthorizeRoles("Administrator")]
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