using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace extreemt.Controllers
{
    public class RegisteredController : Controller
    {
        private Controllers.RegisteredController registerController;
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
            extreemt.user user = extreemt.Account.staticGetLoggedUser();
            extreemt.extreemtEntities db = new extreemt.extreemtEntities();
            List<extreemt.user> allOldUsers = null;
            if (db.users.Where(u => u.registerDate > user.registerDate).Count() > 0)
            {
                allOldUsers = db.users.Where(u => u.registerDate > user.registerDate).OrderByDescending(o => o.registerDate).ToList();
            }
            Account acc = new Account();
            List<user> allRegisterdChilds = acc.getParentChildren(user, allOldUsers);
            ViewData["allRegisterdChilds"] = allRegisterdChilds;
            return View();
        }

        //After_Login My Signup 

        public ActionResult Daily_Signup()
        {
            extreemt.user user = extreemt.Account.staticGetLoggedUser();
            extreemt.extreemtEntities db = new extreemt.extreemtEntities();
            List<extreemt.user> allOldUsers = null;
            if (db.users.Where(u => u.registerDate > user.registerDate).Count() > 0)
            {
                allOldUsers = db.users.Where(u => u.registerDate > user.registerDate && u.registerDate == DateTime.Now).OrderByDescending(o => o.registerDate).ToList();
            }
            Account acc = new Account();
            List<user> allRegisterdChilds = acc.getParentChildren(user, allOldUsers);
            ViewData["allRegisterdChilds"] = allRegisterdChilds;

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
        public ActionResult Update_Info()
        {

            return View();

        }


    }
}
