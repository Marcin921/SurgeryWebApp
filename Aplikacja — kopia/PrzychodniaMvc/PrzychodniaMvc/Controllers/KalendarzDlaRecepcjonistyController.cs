using DHTMLX.Scheduler;
using DHTMLX.Scheduler.Data;
using PrzychodniaMvc.Models.BazaDanych;
using PrzychodniaMvc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Controllers
{
    public class KalendarzDlaRecepcjonistyController : Controller
    {
        public static int LekId;
        public static int PacId;
        // GET: KalendarzDlaRecepcjonisty
        public ActionResult Index()
        {
            var scheduler = new DHXScheduler();
            scheduler.Config.drag_create = false;
            scheduler.Config.drag_lightbox = false;
            scheduler.Config.drag_resize = false;
            scheduler.Config.dblclick_create = false;
            scheduler.Config.separate_short_events = true;
            BlokujWeekendy(scheduler);

            /*
             * It's possible to use different actions of the current controller
             *      var scheduler = new DHXScheduler(this);     
             *      scheduler.DataAction = "ActionName1";
             *      scheduler.SaveAction = "ActionName2";
             * 
             * Or to specify full paths
             *      var scheduler = new DHXScheduler();
             *      scheduler.DataAction = Url.Action("Data", "Calendar");
             *      scheduler.SaveAction = Url.Action("Save", "Calendar");
             */

            /*
             * The default codebase folder is ~/Scripts/dhtmlxScheduler. It can be overriden:
             *      scheduler.Codebase = Url.Content("~/customCodebaseFolder");
             */


            scheduler.InitialDate = DateTime.Today;

            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            scheduler.Skin = DHXScheduler.Skins.Flat;
            scheduler.Config.hour_size_px = 180;
            //scheduler.GenerateJS();

           // scheduler.Config.icons_select = new EventButtonList { EventButtonList.Delete };
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            //SpecjalizacjaLekarza sl = dc.SpecjalizacjaLekarza.FirstOrDefault(sll => sll.Lekarz.Uzytkownik.IdUzytkownika == 8);

            var ListaLekarzySpec = dc.SpecjalizacjaLekarza.OrderBy(sl => sl.Specjalizacja.NazwaSpecjalizacji);
            List<SelectListItem> ListaLekarzy = new List<SelectListItem>();
            ListaLekarzy.Add(new SelectListItem
            {
                Selected = true,
                Text = "Wszyscy Lekarze",
                Value = "0"
            });
            foreach (SpecjalizacjaLekarza sl in ListaLekarzySpec)
            {
                ListaLekarzy.Add(new SelectListItem
                {
                    Selected = false,
                    Text = sl.Lekarz.Imie + " " + sl.Lekarz.Nazwisko + " - " + sl.Specjalizacja.NazwaSpecjalizacji,
                    Value = sl.Lekarz.IdLekarza.ToString()
                });
            }


            var ListaPacjentow = dc.Pacjent.OrderBy(sl => sl.Nazwisko);
            List<SelectListItem> ListaPacjentowSel = new List<SelectListItem>();
            ListaPacjentowSel.Add(new SelectListItem
            {
                Selected = true,
                Text = "Żaden",
                Value = "0"
            });
            foreach (Pacjent p in ListaPacjentow)
            {
                ListaPacjentowSel.Add(new SelectListItem
                {
                    Selected = false,
                    Text = p.Imie + " " + p.Nazwisko + " - pesel:" + p.Pesel,
                    Value = p.IdPacjenta.ToString()
                });
            }

            ViewBag.ListaPacjentow = ListaPacjentowSel;
            ViewBag.ListaLekarzy = ListaLekarzy;
            //var ListaLekarzy = new SelectListItem();

            return View(scheduler);
        }
        [AuthorizeRoles("Recepcjonista")]
        public ContentResult Data()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            List<object> items = new List<object>();
            if (LekId == 0 && PacId == 0)
            {
                var items_blue = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && dc.Wizyta.
                                                        Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() == 0).
                                                Select(t => new {
                                                    id = t.IdRejestracji,
                                                    text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                           "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                           "\n" + "Oczekiwanie na akceptacje",
                                                    start_date = t.DataRozp,
                                                    end_date = t.DataZak,
                                                    color = "blue",
                                                    type = "dhx_time_block"
                                                });
                var items_yellow = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && dc.Wizyta.
                                                              Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() > 0).
                                                  Select(t => new {
                                                      id = t.IdRejestracji,
                                                      text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                            "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                            "\n" + "Wizyta zaakceptowana. Zapraszamy!",
                                                      start_date = t.DataRozp,
                                                      end_date = t.DataZak,
                                                      color = "yellow",
                                                      type = "dhx_time_block"
                                                  });
                var items_green = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("N")).
                                                 Select(t => new {
                                                     id = t.IdRejestracji,
                                                     text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko
                                                            + "\n" + "Termin wolny",
                                                     start_date = t.DataRozp,
                                                     end_date = t.DataZak,
                                                     color = "green",
                                                     type = "dhx_time_block"
                                                 });
                items.AddRange(items_green);
                items.AddRange(items_blue);
                items.AddRange(items_yellow);
            }
            else if(PacId != 0 && LekId == 0)
            {
                var items_red = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdPacjenta != PacId && dc.Wizyta.Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() >= 0).
                                                Select(t => new {
                                                    id = t.IdRejestracji,
                                                    text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                           "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                           "\n" + "Termin zajety",
                                                    start_date = t.DataRozp,
                                                    end_date = t.DataZak,
                                                    color = "red",
                                                    type = "dhx_time_block"
                                                });
                var items_blue = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdPacjenta == PacId && dc.Wizyta.
                                                        Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() == 0).
                                                Select(t => new {
                                                    id = t.IdRejestracji,
                                                    text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                           "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                           "\n" + "Oczekiwanie na akceptacje",
                                                    start_date = t.DataRozp,
                                                    end_date = t.DataZak,
                                                    color = "blue",
                                                    type = "dhx_time_block"
                                                });
                var items_yellow = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdPacjenta == PacId && dc.Wizyta.
                                                              Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() > 0).
                                                  Select(t => new {
                                                      id = t.IdRejestracji,
                                                      text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                            "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                            "\n" + "Wizyta zaakceptowana. Zapraszamy!",
                                                      start_date = t.DataRozp,
                                                      end_date = t.DataZak,
                                                      color = "yellow",
                                                      type = "dhx_time_block"
                                                  });
                var items_green = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("N")).
                                                 Select(t => new {
                                                     id = t.IdRejestracji,
                                                     text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko
                                                            + "\n" + "Termin wolny",
                                                     start_date = t.DataRozp,
                                                     end_date = t.DataZak,
                                                     color = "green",
                                                     type = "dhx_time_block"
                                                 });
                items.AddRange(items_green);
                items.AddRange(items_blue);
                items.AddRange(items_red);
                items.AddRange(items_yellow);
            }
            else if (PacId != 0 && LekId != 0)
            {
                var items_red = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdPacjenta != PacId && rr.IdLekarza != LekId && dc.Wizyta.Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() >= 0).
                                                Select(t => new {
                                                    id = t.IdRejestracji,
                                                    text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                           "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                           "\n" + "Termin zajety",
                                                    start_date = t.DataRozp,
                                                    end_date = t.DataZak,
                                                    color = "red",
                                                    type = "dhx_time_block"
                                                });
                var items_blue = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdPacjenta == PacId && rr.IdLekarza != LekId && dc.Wizyta.
                                                        Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() == 0).
                                                Select(t => new {
                                                    id = t.IdRejestracji,
                                                    text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                           "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                           "\n" + "Oczekiwanie na akceptacje",
                                                    start_date = t.DataRozp,
                                                    end_date = t.DataZak,
                                                    color = "blue",
                                                    type = "dhx_time_block"
                                                });
                var items_yellow = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdPacjenta == PacId && rr.IdLekarza != LekId && dc.Wizyta.
                                                              Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() > 0).
                                                  Select(t => new {
                                                      id = t.IdRejestracji,
                                                      text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                            "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                            "\n" + "Wizyta zaakceptowana. Zapraszamy!",
                                                      start_date = t.DataRozp,
                                                      end_date = t.DataZak,
                                                      color = "yellow",
                                                      type = "dhx_time_block"
                                                  });
                var items_green = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("N") && rr.IdLekarza != LekId).
                                                 Select(t => new {
                                                     id = t.IdRejestracji,
                                                     text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko
                                                            + "\n" + "Termin wolny",
                                                     start_date = t.DataRozp,
                                                     end_date = t.DataZak,
                                                     color = "green",
                                                     type = "dhx_time_block"
                                                 });
                items.AddRange(items_green);
                items.AddRange(items_blue);
                items.AddRange(items_red);
                items.AddRange(items_yellow);
            }
            else if (LekId != 0 && PacId == 0)
            {
                var items_blue = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdLekarza == LekId && dc.Wizyta.
                                                        Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() == 0).
                                                Select(t => new {
                                                    id = t.IdRejestracji,
                                                    text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                           "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                           "\n" + "Oczekiwanie na akceptacje",
                                                    start_date = t.DataRozp,
                                                    end_date = t.DataZak,
                                                    color = "blue",
                                                    type = "dhx_time_block"
                                                });
                var items_yellow = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdLekarza == LekId && dc.Wizyta.
                                                              Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() > 0).
                                                  Select(t => new {
                                                      id = t.IdRejestracji,
                                                      text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                            "\n" + "Pacjent: " + t.Pacjent.Imie + " " + t.Pacjent.Nazwisko +
                                                           "\n" + "Pesel: " + t.Pacjent.Pesel +
                                                            "\n" + "Wizyta zaakceptowana. Zapraszamy!",
                                                      start_date = t.DataRozp,
                                                      end_date = t.DataZak,
                                                      color = "yellow",
                                                      type = "dhx_time_block"
                                                  });
                var items_green = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("N") && rr.IdLekarza == LekId).
                                                 Select(t => new {
                                                     id = t.IdRejestracji,
                                                     text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko
                                                            + "\n" + "Termin wolny",
                                                     start_date = t.DataRozp,
                                                     end_date = t.DataZak,
                                                     color = "green",
                                                     type = "dhx_time_block"
                                                 });
                items.AddRange(items_green);
                items.AddRange(items_blue);
                items.AddRange(items_yellow);
            }



            /*   var items_red = from r in dc.Rejestracja
                            join w in dc.Wizyta on r.IdRejestracji equals w.IdRejestracji into RecWiz
                            from w in RecWiz.DefaultIfEmpty()
                            select
                            new { id = r.IdRejestracji, text = r.IdLekarza, start_date = r.DataRozp, end_date = r.DataZak, color =  "red"};*/
            var data = new SchedulerAjaxData(items);

            return (ContentResult)data;
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult Anuluj(int? id, string color, string anuluj)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();

            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            if (r.CzyZajeta.Equals("Y") && color.Equals("yellow"))
            {
                dc.Wizyta.Remove(dc.Wizyta.FirstOrDefault(ww => ww.IdRejestracji == r.IdRejestracji));
                dc.SaveChanges();
            }
            else if(r.CzyZajeta.Equals("Y") && color.Equals("blue"))
            {
                r.CzyZajeta = "N";
                r.IdPacjenta = null;
                dc.SaveChanges();
            }
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult RezerwujTermin(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            r.CzyZajeta = "Y";
            r.IdPacjenta = PacId;
            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult ZatwierdzTermin(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            Wizyta w = new Wizyta();
            w.IdRejestracji = r.IdRejestracji;
            dc.Wizyta.Add(w);
            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult AnulujAkceptacjeTerminu(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            dc.Wizyta.Remove(dc.Wizyta.FirstOrDefault(ww => ww.IdRejestracji == r.IdRejestracji));
            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult AnulujRezerwacjeTerminu(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            r.CzyZajeta = "N";
            r.IdPacjenta = null;
            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult Zapisz(int? id, string color)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
           
            //var items = dc.Rejestracja.Select(t => new { id = t.IdRejestracji, text = t.IdLekarza, start_date = t.DataRozp, end_date = t.DataZak });
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
           
            if (r.CzyZajeta.Equals("N") && PacId != 0)
            {
                r.CzyZajeta = "Y";
                r.IdPacjenta = PacId;
                dc.SaveChanges();
            }
            else if(r.CzyZajeta.Equals("Y") && color.Equals("yellow"))
            {
                dc.Wizyta.Remove(dc.Wizyta.FirstOrDefault(ww => ww.IdRejestracji == r.IdRejestracji));
                dc.SaveChanges();
            }
            else if (r.CzyZajeta.Equals("Y") && color.Equals("blue"))
            {
                Wizyta w = new Wizyta();
                w.IdRejestracji = r.IdRejestracji;
                dc.Wizyta.Add(w);
                dc.SaveChanges();
            }

            return RedirectToAction("Data");
        }

        [AuthorizeRoles("Recepcjonista")]
        public ActionResult UtworzKalendarz()
        {
            return Redirect("~/KalendarzDlaRecepcjonisty/Index");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult ZmianaIdPacjenta(int? pacid)
        {
            PacId = (int)pacid;
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        public ActionResult ZmianaIdLekarza(int? lekid)
        {
            LekId = (int)lekid;
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Recepcjonista")]
        private void BlokujWeekendy(DHXScheduler scheduler)
        {

            scheduler.TimeSpans.Add(new DHXBlockTime()
            {
                Day = DayOfWeek.Sunday //blocks each Sunday
            });
            scheduler.TimeSpans.Add(new DHXBlockTime()
            {
                Day = DayOfWeek.Saturday //blocks each Sunday
            });
        }
    }
}