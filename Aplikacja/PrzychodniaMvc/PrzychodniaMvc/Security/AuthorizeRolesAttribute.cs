using PrzychodniaMvc.Models.BazaDanych;
using PrzychodniaMvc.Models.ManagerBazyDanych;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Security
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly string[] userAssignedRoles;
        public AuthorizeRolesAttribute(params string[] roles)
        {
            this.userAssignedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool zautoryzowany = false;
            using (PrzychodniaBDEntities7 db = new PrzychodniaBDEntities7())
            {
                ManagerUzytkownika MU = new ManagerUzytkownika();
                foreach (var roles in userAssignedRoles)
                {
                    zautoryzowany = MU.SprawdzDostepDoRoli(httpContext.User.Identity.Name, roles);
                    if (zautoryzowany)
                        return zautoryzowany;
                }
            }
            return zautoryzowany;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/UnAuthorized");
        }

        public static string CheckUserRole(string name)
        {
            PrzychodniaBDEntities7 db = new PrzychodniaBDEntities7();
            return db.RolaUzytkownika.FirstOrDefault(r => r.Uzytkownik.Login == name).Rola.NazwaRoli;
        }
    }

}