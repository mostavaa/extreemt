using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace extreemt.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            string content = Request.QueryString["a"].Replace("25252", "+");
            extreemt.crypt algo = new extreemt.crypt();
            string decryptedId = algo.Decrypt(content);
            string[] user = decryptedId.Split(new char[] { '$', '$', '$' });
            int i = 0;
            while (user[i] == "")
            {
                i++;
            }
            if (user[i] != "2" && user[i] != "3")
                return null;
            ViewData["parentGenNumber"] = user[i];
            i++;
            while (user[i] == "")
            {
                i++;
            }
            if (user[i] != "left" && user[i] != "right")
                return null;
            ViewData["myPos"] = user[i];
            i++;
            while (user[i] == "")
            {
                i++;
            }
            ViewData["parentId"] = user[i];
            ViewData["a"] = content;
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public void submitLogin()
        {
            Account acc = new Account(this);
            Dictionary<string, List<string>> errors = acc.Login();
            if (errors != null)
            {
                foreach (KeyValuePair<string, List<string>> error in errors)
                {
                    Response.Write(error.Key + ",");
                    foreach (string msg in error.Value)
                    {
                        Response.Write(msg);
                    }
                    Response.Write("#");
                }
            }
            else
            {
                Session.Add("userId", Request.Form["id"]);
                Response.Write("success");
            }
        }
        public ActionResult transferMoney()
        {
            
            string error = "";
            try
            {
                extreemtEntities db = new extreemtEntities();
                int transId = int.Parse(Request.Form["id"]);
                if (db.users.Where(u => u.userId == transId && u.genNumber == 1).Count() >= 0){


                    Account acc = new Account(this);
                    user loggedUser = acc.getLoggedUser();

                    /*
                    //temp                    
                    Session.Add("userId", "25478961");
                    int currentuserId = int.Parse(Session["userId"].ToString());
                    */
                    int amount = int.Parse(Request.Form["amount"]);
                    if (db.users.Where(u => u.userId == loggedUser.userId && u.genNumber == 1).Count() >= 0)
                    {

                        user transUser = db.users.Where(u => u.userId == transId && u.genNumber == 1).First();
                        if (loggedUser.userId != transUser.userId)
                        {
                            if (loggedUser.cashBank >= amount)
                            {
                                db.Entry(loggedUser).State = System.Data.EntityState.Modified;
                                db.Entry(transUser).State = System.Data.EntityState.Modified;
                                transUser.cashBank += amount;
                                loggedUser.cashBank -= amount;
                                db.SaveChanges();
                                error += "Amount Transfered Successfully";
                            }
                            else
                            {
                                error += "You Don't Have Enough Credit !";
                            }
                        }
                        else
                        {
                            if (loggedUser.cashBank >= amount)
                            {
                                loggedUser.productBank += amount;
                                loggedUser.cashBank -= amount;
                                db.Entry(transUser).CurrentValues.SetValues(loggedUser);
                                db.SaveChanges();
                                error += "Amount Transfered Successfully to Your Product Bank!";                            
                            }
                        }
                    }
                    else
                    {
                        error += "You Aren't Login";
                    }
                }
                else
                {
                    error += "No User With This Id";
                }
            }
            catch (Exception e)
            {
                error += "Error Occured , please make sure that you provide a VALID ID and AMOUNT";
            }
            TempData["error"] = error;
            return Redirect(Url.Action("transfer", "Account"));
             
        }
        public ActionResult transfer()
        {
            ViewData["error"] = TempData["error"];
            return View();
        }
        public void submitSignUp()
        {

            Account acc = new Account(this);
            Dictionary<string, List<string>> errors = acc.signUp();
            if (errors != null)
            {
                foreach (KeyValuePair<string, List<string>> error in errors)
                {
                    Response.Write(error.Key + ",");
                    foreach (string msg in error.Value)
                    {
                        Response.Write(msg);
                        
                    }
                    Response.Write("#");
                }
            }
            else
            {
                string userId = (string)Session["userId"];
                if (userId != null && userId != "")
                {
                    extreemt.crypt algo = new extreemt.crypt();
                    userId = algo.Encrypt(userId + "$$$1");
                    userId = HttpUtility.UrlEncode(userId.Replace("+", "25252"));
                }
                string url = Url.Action("genology", "Home") + "?a=" + userId;         
                //Response.Redirect(url);

                Response.Write("success," + url);
            }

        }

        public void getParent()
        {
            /*
            int parentId = int.Parse(Request.Form["id"]);
            extreemtEntities db = new extreemtEntities();
            if (db.users.Where(o => o.userId == parentId).Count() > 0)
            {
                Response.Write(db.users.Where(o => o.userId == parentId).First().firstname);
            }
             */ 
        }

        // new 
        public ActionResult Admin()
        {
            ViewData["adminCashBank"] = getAdminCredit();
            ViewData["SignUpStatus"] = getSignUpStatus();


            return View();
        }


        public void changeAdminCredit()
        {
            int newval = int.Parse (Request.Form["val"]);

            Account acc = new Account(this);
            acc.changeAdminCredit(newval); 
            Response.Write("success");
        }

        public int getAdminCredit()
        {

            Account acc = new Account(this);
            int adminCredit = acc.getAdminCredit();
            return adminCredit;
        }
        public void changeSignUp()
        {
            bool status = bool.Parse(Request.Form["status"]);
            Account acc = new Account(this);
            acc.changeSignUp(status);
            Response.Write("success");
        }
        public bool getSignUpStatus()
        {
            Account acc = new Account(this);
            return acc.signUpClosed();   
        }
        public void logout()
        {
            Session["userId"] = "0";
            Response.Redirect("~");
        }

        public void getUserByIdForTransfer()
        {
            int id = int.Parse(Request.Form["id"]);
            Account acc = new Account(this);
            user user = acc.getUserById(id);
            if (user != null)
            {
                Response.Write(user.username);
            }
        }

    }
}
