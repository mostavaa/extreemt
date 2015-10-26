using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace extreemt.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Profile()
        {
            return View();
        }
        public ActionResult genology()
        {
            ViewData["id"] = Request.QueryString["a"].Replace("25252", "+");
            return View();
        }
    }
}
