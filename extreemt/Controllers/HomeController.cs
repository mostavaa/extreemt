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

<<<<<<< HEAD
        }
        public ActionResult Category()
        {
            return View();
        }
      
        public ActionResult Profile()
        {
            return View();
        }
        public  ActionResult Update_Category()
        {
            return View();
=======
>>>>>>> 1552016cd5186ef7b43dba1bcf613068a253d068

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
