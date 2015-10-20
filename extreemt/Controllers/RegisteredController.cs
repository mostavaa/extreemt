using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace extreemt.Controllers
{
    public class RegisteredController : Controller
    {
        //
        // GET: /Registered/

        public ActionResult Index()
        {
            return View();
        }
        //After_Login  Cash Commission Dropdown
        public ActionResult Cash_Bank()
        {

            return View();

        }
        public ActionResult Product_Bank()
        {
            return View();
        }
        public ActionResult Account_Summary()
        {
            return View();
        }

        //After_Login My Team Dropdown

        public ActionResult Group_list()
        {
            return View();
        }

        //After_Login My Signup 

        public ActionResult Daily_Signup()
        {
            return View();
        }

        //After_Login Transfer

        public ActionResult My_Transfer()
        {
            return View();

        }
        //After_Login My Profile

        public ActionResult My_Basket()
        {
            return View();

        }
        public ActionResult My_Order()
        {
            return View();
        }


        }
}
