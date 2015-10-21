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
         public ActionResult Update_Info()
        {
           
             return View();

        }
        public ActionResult Sumbit_edit()
         {
            extreemtEntities db = new extreemtEntities();

            int userid = int.Parse(System.Web.HttpContext.Current.Session["userId"].ToString());
           List<user> usrs = db.users.Where(u => u.userId == userid).ToList();
            // Validate Input 
           bool change = false;
           if (Request.Form["loginPassword"] != null || Request.Form["loginPassword"] != "")
           {
               if (usrs[0].loginPassword == Request.Form["loginPassword"])
               {
                   if (Request.Form["newLoginPassword"] == Request.Form["confirmLoginPassword"])
                       change = true;
                   else
                   {
                       Response.Write("New Login And Confirm Login Dosn't Match ");
                   }

               }
               else
               {
                   Response.Write("Login Password Error ");

               }
           }
               
              
           foreach (var usr in usrs)
           {
               if(change == true)
                   usr.loginPassword = Request.Form["newLoginPassword"];
               usr.mail = Request.Form["mail"];
               usr.title = Request.Form["title"];
               usr.username = Request.Form["username"];
               usr.mobile = Request.Form["mobile"];
               usr.homephone = Request.Form["homephone"];
               usr.country = Request.Form["country"];
               usr.city = Request.Form["city"];
               usr.address = Request.Form["address"];
               usr.ssn = Request.Form["ssn"];
               usr.nationality = Request.Form["nationality"];
               usr.relationship = Request.Form["relationship"];

            
               db.Entry(usr).State = EntityState.Modified;

           }
             //db.Entry(usr).State = EntityState.Modified;

             db.SaveChanges();

             return RedirectToAction("Update_Info", "Registered"); 
         }

        }
}
