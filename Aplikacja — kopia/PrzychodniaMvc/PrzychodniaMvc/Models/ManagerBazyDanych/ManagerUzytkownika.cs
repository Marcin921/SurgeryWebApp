using PrzychodniaMvc.Models.BazaDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrzychodniaMvc.Models.ManagerBazyDanych
{
    public class ManagerUzytkownika
    {
        public bool SprawdzDostepDoRoli(string login, string nazwaRoli)
        {
            using (PrzychodniaBDEntities7 db = new PrzychodniaBDEntities7())
            {
                Uzytkownik u = db.Uzytkownik.Where(o => o.Login.ToLower().Equals(login))?.FirstOrDefault();
                if (u != null)
                {
                    var roles = from q in db.RolaUzytkownika
                                join r in db.Rola on q.IdRoli equals r.IdRoli
                                where r.NazwaRoli.Equals(nazwaRoli) && q.IdUzytkownika.Equals(u.IdUzytkownika)
                                select r.NazwaRoli;

                    if (roles != null)
                    {
                        return roles.Any();
                    }
                }

                return false;
            }
        }
    }
}