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
        public void taskscheduledforcommmission()
        {
            extreemtEntities db = new extreemtEntities();
            if (db.users.Where(u => u.isActive && u.dailyLeftActiveCount != 0 && u.dailyRightActiveCount != 0).Count() > 0)
            {
                List<user> users = db.users.Where(u => u.isActive && u.dailyLeftActiveCount != 0 && u.dailyRightActiveCount != 0).ToList();
                foreach (user usr in users)
                {
                    int chequesCount = usr.cheques.Count;
                    if (usr.genNumber == 1)
                    {
                        chequesCount += db.users.Where(u => u.parentUserId == usr.userId && u.genNumber == 2).First().cheques.Count;
                        chequesCount += db.users.Where(u => u.parentUserId == usr.userId && u.genNumber == 3).First().cheques.Count;
                    }
                    else if(usr.genNumber==2)
                    {
                        user parent = db.users.Where(u => u.userId == usr.parentUserId && u.genNumber == 1).First();
                        chequesCount += parent.cheques.Count;
                      chequesCount+= db.users.Where(u => u.parentUserId == parent.userId && u.genNumber == 3).First().cheques.Count;
                    }
                    else
                    {
                        user parent = db.users.Where(u => u.userId == usr.parentUserId && u.genNumber == 1).First();
                        chequesCount += parent.cheques.Count;
                        chequesCount += db.users.Where(u => u.parentUserId == parent.userId && u.genNumber == 2).First().cheques.Count;
                    
                    }
                    int dailyLeftCount = usr.dailyLeftActiveCount;
                    int dailyRightCount = usr.dailyRightActiveCount;
                    int numberOfCheques = 0;
                    if (dailyLeftCount >= dailyRightCount)
                    {
                        numberOfCheques = dailyRightCount;
                        usr.dailyLeftActiveCount = usr.dailyLeftActiveCount - usr.dailyRightActiveCount;
                        usr.dailyRightActiveCount = 0;
                    }
                    else
                    {
                        numberOfCheques = dailyLeftCount;
                        usr.dailyRightActiveCount = usr.dailyRightActiveCount - usr.dailyLeftActiveCount;
                        usr.dailyLeftActiveCount = 0;
                    }
                    if (numberOfCheques > 6)
                    {
                        numberOfCheques = 6;
                    }

                    int productAdded = 0, cashAdded = 0;
                    for (int i = 0; i < numberOfCheques; i++)
                    {
                        cheque ch = new cheque();
                        ch.amount = 250;
                        ch.chequeDate = DateTime.Now;
                        ch.userId = usr.id;
                        if (((i + chequesCount) + 1) % 3 == 0)
                        {
                            ch.chequeType = "product";
                            productAdded += 250;
                        }
                        else
                        {
                            ch.chequeType = "cash";
                            cashAdded += 250;
                        }
                        db.cheques.Add(ch);
                    }
                    if (usr.genNumber == 1)
                    {

                        usr.cashBank += cashAdded;
                        usr.productBank += productAdded;

                        db.Entry(usr).State = System.Data.EntityState.Modified;
                    }
                    else
                    {
                        user parent = db.users.Where(u => u.userId == usr.parentUserId).First();
                        parent.cashBank += cashAdded;
                        parent.productBank += productAdded;

                        db.Entry(parent).State = System.Data.EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
        }
        public ActionResult transferMoney()
        {

            string error = "";
            try
            {
                extreemtEntities db = new extreemtEntities();
                int transId = int.Parse(Request.Form["id"]);
                if (db.users.Where(u => u.userId == transId && u.genNumber == 1).Count() >= 0)
                {


                    Account acc = new Account(this);
                    user logged = Account.staticGetLoggedUser();

                    /*
                    //temp                    
                    Session.Add("userId", "25478961");
                    int currentuserId = int.Parse(Session["userId"].ToString());
                    */
                    int amount = int.Parse(Request.Form["amount"]);
                    if (db.users.Where(u => u.userId == logged.userId && u.genNumber == 1).Count() >= 0)
                    {

                        user transUser = db.users.Where(u => u.userId == transId && u.genNumber == 1).First();
                        if (logged.userId != transUser.userId)
                        {
                            if (logged.cashBank >= amount)
                            {
                                user loggedUser = db.users.Find(logged.id);

                                db.Entry(loggedUser).State = System.Data.EntityState.Modified;
                                db.Entry(transUser).State = System.Data.EntityState.Modified;
                                transUser.cashBank += amount;
                                loggedUser.cashBank -= amount;
                                db.SaveChanges();
                                error += "Amount Transfered Successfully";
                                Transfer trans = new Transfer();
                                trans.transferFrom = loggedUser.id;
                                trans.transferTo = transUser.id;
                                trans.transferDate = DateTime.Now;
                                trans.amount = amount;
                                db.Transfers.Add(trans);
                                db.SaveChanges();
                            }
                            else
                            {
                                error += "You Don't Have Enough Credit !";
                            }
                        }
                        else
                        {
                            if (logged.cashBank >= amount)
                            {
                                logged.productBank += amount;
                                logged.cashBank -= amount;
                                db.Entry(transUser).CurrentValues.SetValues(logged);
                                db.SaveChanges();
                                error += "Amount Transfered Successfully to Your Product Bank!";
                                Transfer trans = new Transfer();
                                trans.transferFrom = logged.id;
                                trans.transferTo = logged.id;
                                trans.transferDate = DateTime.Now;
                                trans.amount = amount;
                                db.Transfers.Add(trans);
                                db.SaveChanges();
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
        public void Sumbit_edit()
        {

            Account acc = new Account(this);
            Dictionary<string, List<string>> errors = acc.UpdateInfo();
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
            int newval = int.Parse(Request.Form["val"]);

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
