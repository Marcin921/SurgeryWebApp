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
    public class RecepcjonistaController : Controller
    {
        // GET: Recepcjonista
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult WeryfikujPacjentow()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var p = dc.Pacjent.OrderByDescending(pp => pp.Zatwierdzono ).ToList();
            return View(p);
        }

        public ActionResult WeryfikujPacjenta(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Pacjent p = dc.Pacjent.FirstOrDefault(t => t.IdPacjenta == id);
            p.Zatwierdzono = true;
            dc.SaveChanges();
            return RedirectToAction("WeryfikujPacjentow");
        }

        public ActionResult CofnijWeryfikacjePacjenta(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Pacjent p = dc.Pacjent.FirstOrDefault(t => t.IdPacjenta == id);
            p.Zatwierdzono = false;
            dc.SaveChanges();
            return RedirectToAction("WeryfikujPacjentow");
        }

        [AuthorizeRoles("Recepcjonista")]
        public ActionResult EdytujPacjenta(Pacjent p, Nullable<int> id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            Pacjent pp = dc.Pacjent.FirstOrDefault(ppp => ppp.IdPacjenta == id);
            if (ModelState.IsValid)
            {
                pp.Imie = p.Imie;
                pp.Nazwisko = p.Nazwisko;
                pp.KodPocztowy = p.KodPocztowy;
                pp.Miasto = p.Miasto;
                pp.NumerTelefonu = p.NumerTelefonu;
                pp.Pesel = p.Pesel;
                pp.Zatwierdzono = p.Zatwierdzono;
                dc.SaveChanges();
            }
            return View(pp);
        }

        [AuthorizeRoles("Recepcjonista")]
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

        public ActionResult Zaloguj()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Zaloguj(Recepcjonista r)
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
                    ViewBag.BladLogowania = true;
                }
                return View(r);
        }
        public ActionResult Wyloguj()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}