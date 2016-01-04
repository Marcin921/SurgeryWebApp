using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using DHTMLX.Scheduler.Controls;

using PrzychodniaMvc.Models;
using PrzychodniaMvc.Models.BazaDanych;
using System.Data;
using PrzychodniaMvc.Security;

namespace PrzychodniaMvc.Controllers
{
    public class KalendarzDlaPacjentaController : Controller
    {
        public static int LekId;
        public static int PacId;

        [AuthorizeRoles("Pacjent")]
        public ActionResult Index()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            //Being initialized in that way, scheduler will use CalendarController.Data as a the datasource and CalendarController.Save to process changes
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

            scheduler.Config.icons_select = new EventButtonList { };
           
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
           
            ViewBag.ListaLekarzy = ListaLekarzy;
            //var ListaLekarzy = new SelectListItem();

           return View(scheduler);        
        }
        [AuthorizeRoles("Pacjent")]
        public ActionResult UtworzKalendarz()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            PacId = dc.Pacjent.FirstOrDefault(pp => pp.Uzytkownik.Login == HttpContext.User.Identity.Name).IdPacjenta;
            return Redirect("~/KalendarzDlaPacjenta/Index");
        }
        [AuthorizeRoles("Pacjent")]
        public ActionResult ZmianaIdLekarza(int? lekid)
        {
            LekId = (int) lekid;
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Pacjent")]
        public ActionResult RezerwujTermin(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            r.CzyZajeta = "Y";
            r.IdPacjenta = PacId;
            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        public ActionResult AnulujRezerwacjeTerminu(int? id)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            r.CzyZajeta = "N";
            r.IdPacjenta = null;
            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Pacjent")]
        public ContentResult Data()
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            List<object> items = new List<object>();
            if ( LekId == 0)
            {
                var items_red = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.Pacjent.IdPacjenta != PacId && rr.DataRozp >= DateTime.Now).
                                               Select(t => new { id = t.IdRejestracji, text ="Lekarz: " + t.Lekarz.Imie + " "  + t.Lekarz.Nazwisko + 
                                                                "\n" +  "Termin zajety", start_date = t.DataRozp, end_date = t.DataZak,
                                                                color = "red", type = "dhx_time_block" });
                var items_blue = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.Pacjent.IdPacjenta == PacId && rr.DataRozp >= DateTime.Now && dc.Wizyta.
                                                        Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() == 0).
                                                Select(t => new { id = t.IdRejestracji, text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                                  "\n" + "Oczekiwanie na akceptacje", start_date = t.DataRozp, end_date = t.DataZak,
                                                                  color = "blue", type = "dhx_time_block" });
                var items_yellow = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.Pacjent.IdPacjenta == PacId && rr.DataRozp >= DateTime.Now && dc.Wizyta.
                                                              Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() > 0).
                                                  Select(t => new { id = t.IdRejestracji, text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                                   "\n" + "Wizyta zaakceptowana. Zapraszamy!", start_date = t.DataRozp,
                                                                    end_date = t.DataZak, color = "yellow", type = "dhx_time_block" });
                var items_green = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("N") && rr.DataRozp >= DateTime.Now).
                                                 Select(t => new { id = t.IdRejestracji, text = "Lekarz: " + t.Lekarz.Imie + " " + t.Lekarz.Nazwisko 
                                                                   + "\n" + "Termin wolny", start_date = t.DataRozp, end_date = t.DataZak, color = "green",
                                                                   type = "dhx_time_block" });
                items.AddRange(items_green);
                items.AddRange(items_blue);
                items.AddRange(items_red);
                items.AddRange(items_yellow);
            }
            else
            {
                //var items = dc.Rejestracja.Select(t => new { id = t.IdRejestracji, text = t.IdLekarza, start_date = t.DataRozp, end_date = t.DataZak });
                 var items_red = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdLekarza == LekId && rr.Pacjent.IdPacjenta != PacId && rr.DataRozp >= DateTime.Now).
                                                Select(t => new { id = t.IdRejestracji, text = "Termin zajety", start_date = t.DataRozp, end_date = t.DataZak,
                                                                  color = "red", type = "dhx_time_block" });

                var items_blue = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdLekarza == LekId && rr.Pacjent.IdPacjenta == PacId && rr.DataRozp >= DateTime.Now && dc.Wizyta.
                                                       Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() == 0).
                                               Select(t => new { id = t.IdRejestracji, text = t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                                 "\n" + "Oczekiwanie na akceptacje", start_date = t.DataRozp, end_date = t.DataZak,
                                                                 color = "blue", type = "dhx_time_block"});

                var items_yellow = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("Y") && rr.IdLekarza == LekId && rr.Pacjent.IdPacjenta == PacId && rr.DataRozp >= DateTime.Now && dc.Wizyta.
                                                              Where(w => w.Rejestracja.IdRejestracji == rr.IdRejestracji).Count() > 0).
                                                  Select(t => new { id = t.IdRejestracji, text = t.Lekarz.Imie + " " + t.Lekarz.Nazwisko +
                                                                   "\n" + "Wizyta zaakceptowana. Zapraszamy!", start_date = t.DataRozp, end_date = t.DataZak,
                                                                   color = "yellow", type = "dhx_time_block"});

                var items_green = dc.Rejestracja.Where(rr => rr.CzyZajeta.Equals("N") && rr.IdLekarza == LekId && rr.DataRozp >= DateTime.Now).
                                                 Select(t => new { id = t.IdRejestracji, text = "Termin wolny", start_date = t.DataRozp, end_date = t.DataZak,
                                                                  color = "green", type = "dhx_time_block" });
                items.AddRange(items_green);
                items.AddRange(items_blue);
                items.AddRange(items_red);
                items.AddRange(items_red);
            }
               
           
            /*   var items_red = from r in dc.Rejestracja
                            join w in dc.Wizyta on r.IdRejestracji equals w.IdRejestracji into RecWiz
                            from w in RecWiz.DefaultIfEmpty()
                            select
                            new { id = r.IdRejestracji, text = r.IdLekarza, start_date = r.DataRozp, end_date = r.DataZak, color =  "red"};*/
            var data = new SchedulerAjaxData(items);

            return (ContentResult)data;
        }
        [AuthorizeRoles("Pacjent")]
        public ActionResult Zapisz(int ?id,string color)
        {
            PrzychodniaBDEntities7 dc = new PrzychodniaBDEntities7();
            //var items = dc.Rejestracja.Select(t => new { id = t.IdRejestracji, text = t.IdLekarza, start_date = t.DataRozp, end_date = t.DataZak });
            var r = dc.Rejestracja.FirstOrDefault(rr => rr.IdRejestracji == id);
            if(r.CzyZajeta.Equals("N"))
            {
                r.CzyZajeta = "Y";
                r.IdPacjenta = PacId;
            }
            else if(r.CzyZajeta.Equals("Y") && color.Equals("blue"))
            {
                r.CzyZajeta = "N";
                r.IdPacjenta = null;
            }

            dc.SaveChanges();
            return RedirectToAction("Data");
        }
        [AuthorizeRoles("Pacjent")]
        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);
            
            try
            {
                var changedEvent = (CalendarEvent)DHXEventsHelper.Bind(typeof(CalendarEvent), actionValues);

     

                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        //do insert
                        //action.TargetId = changedEvent.id;//assign postoperational id
                        break;
                    case DataActionTypes.Delete:
                        //do delete
                        break;
                    default:// "update"                          
                        //do update
                        break;
                }
            }
            catch
            {
                action.Type = DataActionTypes.Error;
            }
            return (ContentResult)new AjaxSaveResponse(action);
        }
        [AuthorizeRoles("Pacjent")]
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

