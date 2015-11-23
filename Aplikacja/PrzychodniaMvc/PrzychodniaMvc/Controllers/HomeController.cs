using PrzychodniaMvc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrzychodniaMvc.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizeRoles("Admin")]
        public ActionResult AdminOnly()
        {
            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}