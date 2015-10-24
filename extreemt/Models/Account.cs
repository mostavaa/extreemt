using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace extreemt
{

    public class Account
    {

        private extreemtEntities context;
        //private System.Collections.Specialized.NameValueCollection Form;
        private Controllers.AccountController accountController;
        private Controllers.RegisteredController registeredController;


        public Account(Controllers.AccountController accountController)
        {

            this.accountController = accountController;
            this.context = new extreemtEntities();
        }

        public Account(Controllers.RegisteredController registeredController)
        {
            // TODO: Complete member initialization
            this.registeredController = registeredController;
        }
        public Account()
        {

        }

        public List<user> getParentChildren(user parent, List<user> candidates)
        {
            List<user> children = new List<user>();
            if (candidates == null || candidates.Count == 0)
                return children;
            foreach (user candidate in candidates)
            {
                user can = candidate;
                if (this.isChild(parent, candidate))
                {
                    children.Add(can);
                }
            }
            return children;
        }
        public bool isChild(user parent, user candidate)
        {
            if (candidate.id == parent.id)
            {
                return false;
            }
            extreemtEntities db = new extreemtEntities();
            while (true)
            {
                if (db.users.Where(u => u.userId == candidate.parentUserId).Count() > 0)
                {
                    user candidateParent = db.users.Where(u => u.userId == candidate.parentUserId).First();
                    if (candidateParent.id == parent.id)
                    {
                        return true;
                    }
                    candidate = candidateParent;
                }
                else
                {
                    return false;
                }

            }
        }
        public bool isRight(user parent, user user)
        {
            if (user.id == parent.id)
                return false;

            extreemtEntities db = new extreemtEntities();
            while (true)
            {
                if (user.parentUserId == parent.userId)
                {
                    if (user.position == "right")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (db.users.Where(u => u.userId == user.parentUserId).Count() > 0)
                    user = db.users.Where(u => u.userId == user.parentUserId).First();

            }
        }
        public static bool isAdmin(user user)
        {
            return (user.registererId == null && user.parentUserId == null && user.parentGenNum == 0 && user.genNumber == 1);
        }
        public Dictionary<string, List<string>> Login()
        {
            validateAccount va = new validateAccount(this.accountController.Request.Form);
            if (va.validateLogin().Count > 0)
                return va.validateLogin();

            extreemtEntities db = new extreemtEntities();


            int userId = int.Parse(this.accountController.Request.Form["id"]);
            string password = this.accountController.Request.Form["password"];
            if (db.users.Where(u => u.userId == userId && u.loginPassword == password).Count() <= 0)
            {

                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
                errors.Add("id", new List<string>() { "<p>Id or password may wrong</p>" });
                return errors;
            }
            return null;

        }
        public bool passwordChanged = false;
        private void updateUser()
        {
            extreemtEntities db = new extreemtEntities();

            int userid = int.Parse(System.Web.HttpContext.Current.Session["userId"].ToString());
            List<user> usrs = db.users.Where(u => u.userId == userid).ToList();
            // Validate Input 


            
            foreach (var usr in usrs)
            {
                if (this.passwordChanged == true)
                    usr.loginPassword = this.accountController.Request.Form["newLoginPassword"];
                usr.mail = this.accountController.Request.Form["mail"];
                usr.title = this.accountController.Request.Form["title"];
                usr.username = this.accountController.Request.Form["username"];
                usr.mobile = this.accountController.Request.Form["mobile"];
                usr.homephone = this.accountController.Request.Form["homephone"];
                usr.country = this.accountController.Request.Form["country"];
                usr.city = this.accountController.Request.Form["city"];
                usr.address = this.accountController.Request.Form["address"];
                usr.ssn = this.accountController.Request.Form["ssn"];
                usr.nationality = this.accountController.Request.Form["nationality"];
                usr.relationship = this.accountController.Request.Form["relationship"];


                db.Entry(usr).State = System.Data.EntityState.Modified;

            }
            //db.Entry(usr).State = EntityState.Modified;

            db.SaveChanges();
        }
        public Dictionary<string, List<string>> UpdateInfo()
        {
            validateAccount va = new validateAccount(this.accountController.Request.Form);
            va.mode = "update";
            if (va.validateSignUp().Count > 0)
                return va.validateSignUp();
            this.passwordChanged = va.passwordChanged;
            this.updateUser();
            return null;
        }
        public Dictionary<string, List<string>> signUp()
        {
            validateAccount va = new validateAccount(this.accountController.Request.Form);
            /*
            if (this.signUpClosed())
            {
                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
                errors.Add("Sign Up Closed", new List<string> { "Sign Up Closed Temporarily" });
                return errors;
            }
            */
            if (va.validateSignUp().Count > 0)
                return va.validateSignUp();


            this.insertUser();

            return null;
        }
        private void insertUser()
        {

            string content = this.accountController.Request.Form["a"];

            extreemt.crypt algo = new extreemt.crypt();
            string decryptedId = algo.Decrypt(content);
            string[] userr = decryptedId.Split(new char[] { '$', '$', '$' });
            int i = 0;
            while (userr[i] == "")
            {
                i++;
            }

            string parentGenNumber = userr[i];
            i++;
            while (userr[i] == "")
            {
                i++;
            }

            string myPos = userr[i];
            i++;
            while (userr[i] == "")
            {
                i++;
            }
            string parentIdd = userr[i];





            extreemtEntities db = new extreemtEntities();
            int parId = int.Parse(parentIdd);


            int parGenNumber = int.Parse(parentGenNumber);
            if (db.users.Where(u => u.parentUserId == parId && u.position == myPos && u.parentGenNum == parGenNumber).Count() > 0)
            {
                return;
            }

            string userId = (string)HttpContext.Current.Session["userId"];
            if (userId != null && userId != "")
            {
                int currentUserId = int.Parse(userId);
                user current = db.users.Where(u => u.userId == currentUserId && u.genNumber == 1).First();
                //current.credit -= 250;
                db.Entry(current).State = System.Data.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                return;
            }

            if (!userHaveEnoughMoneyForRegister(this.getLoggedUser()))
            {
                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
                errors.Add("Parent Doesn't Have Enouph Money", new List<string> { "You Don't Have Enouph Money" });
                return;
            }


            user user = new user();
            user.mail = tools.trimMoreThanOneSpace(this.accountController.Request.Form["mail"]);
            user.title = tools.trimMoreThanOneSpace(this.accountController.Request.Form["title"]);
            user.username = tools.trimMoreThanOneSpace(this.accountController.Request.Form["username"]);
            user.mobile = tools.trimMoreThanOneSpace(this.accountController.Request.Form["mobile"]);
            user.homephone = tools.trimMoreThanOneSpace(this.accountController.Request.Form["homephone"]);
            user.country = tools.trimMoreThanOneSpace(this.accountController.Request.Form["country"]);
            user.city = tools.trimMoreThanOneSpace(this.accountController.Request.Form["city"]);
            user.address = tools.trimMoreThanOneSpace(this.accountController.Request.Form["address"]);
            user.ssn = tools.trimMoreThanOneSpace(this.accountController.Request.Form["ssn"]);
            user.nationality = tools.trimMoreThanOneSpace(this.accountController.Request.Form["nationality"]);
            user.relationship = tools.trimMoreThanOneSpace(this.accountController.Request.Form["relationship"]);
            user.loginPassword = tools.trimMoreThanOneSpace(this.accountController.Request.Form["loginPassword"]);
            user.walletPassword = tools.trimMoreThanOneSpace(this.accountController.Request.Form["binCode"]);
            user.parentUserId = int.Parse(parentIdd);
            user.position = myPos;
            user.parentGenNum = int.Parse(parentGenNumber);
            user.genNumber = 1;
            user.status = "inActive";
            user.leftActiveCount = 0;
            user.rightActiveCount = 0;
            user.rightInactiveCount = 1;
            user.leftInactiveCount = 1;
            user.registerDate = DateTime.Now;
            user.registererId = int.Parse(HttpContext.Current.Session["userId"].ToString());

            string _generatedNumber = tools.generateRandomNumber(8);
            int generatedNumber = int.Parse(_generatedNumber);
            while (db.users.Where(o => o.userId == generatedNumber).Count() > 0)
            {
                _generatedNumber = tools.generateRandomNumber(8);
                generatedNumber = int.Parse(_generatedNumber);
            }


            user.userId = generatedNumber;
            db.users.Add(user);

            db.SaveChanges();
            user leftUser = user;
            leftUser.parentGenNum = 1;
            leftUser.parentUserId = user.userId;
            leftUser.genNumber = 2;
            leftUser.position = "left";
            leftUser.leftInactiveCount = 0;
            leftUser.rightInactiveCount = 0;
            leftUser.status = "inActive";

            db.users.Add(leftUser);

            db.SaveChanges();
            user rightUser = user;
            rightUser.parentGenNum = 1;
            rightUser.parentUserId = user.userId;
            rightUser.genNumber = 3;
            rightUser.position = "right";
            rightUser.leftInactiveCount = 0;
            rightUser.rightInactiveCount = 0;
            rightUser.status = "inActive";
            db.users.Add(rightUser);
            db.SaveChanges();

            user parent = db.users.Where(u => u.userId == user.userId && u.genNumber == 1).First();
            this.UpdateParents(parent);
            this.cutOffCashCredit(this.getLoggedUser(), 250);
            //update parents 
        }
        private void cutOffCashCredit(user user, int amount)
        {
            this.context = new extreemtEntities();
            user usr = this.context.users.Find(user.id);
            this.context.Entry(usr).State = System.Data.EntityState.Modified;
            usr.cashBank -= amount;
            this.context.SaveChanges();
        }
        private bool userHaveEnoughMoneyForRegister(user user)
        {
            if (user.cashBank >= 250)
            {
                return true;
            }
            return false;
        }
        private void UpdateParents(user user)
        {
            List<user> parents = new List<extreemt.user>();
            extreemtEntities db = new extreemtEntities();
            user parent = new user();
            while (user.position != "parent")
            {

                parent = db.users.Where(u => u.userId == user.parentUserId && u.genNumber == user.parentGenNum).First();

                if (user.position == "right")
                {
                    parent.rightInactiveCount += 3;
                }
                else if (user.position == "left")
                {
                    parent.leftInactiveCount += 3;
                }
                parents.Add(parent);
                user = new extreemt.user();
                user = parent;
            }
            foreach (user par in parents)
            {
                db.Entry(par).State = System.Data.EntityState.Modified;
            }
            db.SaveChanges();
        }
        public void changeAdminCredit(int newCredit)
        {
            this.context = new extreemtEntities();
            user user = getLoggedUser();
            if (isAdmin(user))
            {
                this.context.Entry(user).State = System.Data.EntityState.Modified;
                user.cashBank = newCredit;
                this.context.SaveChanges();
            }
        }
        public int getAdminCredit()
        {
            user user = getLoggedUser();
            if (isAdmin(user))
            {
                return user.cashBank;
            }
            return 0;
        }
        public void changeSignUp(bool open)
        {
            FileStream fs = new FileStream(this.accountController.Server.MapPath("~") + "/Content/files/isCloseSignUp.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.Write(open.ToString());

            sw.Close();
            fs.Close();
        }
        public bool signUpClosed()
        {
            FileStream fs = new FileStream(this.accountController.Server.MapPath("~") + "/Content/files/isCloseSignUp.txt", FileMode.Open);
            StreamReader sw = new StreamReader(fs);

            string open = sw.ReadToEnd();

            sw.Close();
            fs.Close();

            if (open.ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static bool isUserLogged()
        {
            return (HttpContext.Current.Session["userId"] != "" && HttpContext.Current.Session["userId"] != null);
        }
        public user getUserById(int id)
        {
            extreemtEntities db = new extreemtEntities();
            if (db.users.Where(u => u.userId == id).Count() > 0)
            {
                return db.users.Where(u => u.userId == id && u.genNumber == 1).First();
            }
            return null;
        }
        public user getLoggedUser()
        {
            this.context = new extreemtEntities();
            if (Account.isUserLogged())
            {
                int userid = int.Parse(HttpContext.Current.Session["userId"].ToString());


                try
                {
                    return this.context.users.Where(u => u.userId == userid).First();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return null;
        }




        public static user staticGetLoggedUser()
        {
            extreemtEntities db = new extreemtEntities();
            if (Account.isUserLogged())
            {
                int userid = int.Parse(HttpContext.Current.Session["userId"].ToString());


                try
                {
                    return db.users.Where(u => u.userId == userid).First();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return null;
        }

    }
    public class validateAccount
    {
        private System.Collections.Specialized.NameValueCollection Form;

        private Dictionary<string, List<string>> errors;

        public validateAccount(System.Collections.Specialized.NameValueCollection Form)
        {

            this.Form = Form;
        }
        private void addError(string field, string errorMsg)
        {
            if (this.errors.ContainsKey(field))
            {
                this.errors[field].Add(errorMsg);
            }
            else
            {
                List<string> tempError = new List<string>();
                tempError.Add(errorMsg);
                this.errors.Add(field, tempError);
            }
        }
        private void required(List<string> fields)
        {
            foreach (string field in fields)
            {
                string value = this.Form[field];
                if (value == "" || value == null)
                {
                    addError(field, "<p>" + field.ToUpper() + " is Required</p>");
                    continue;
                }
                value = tools.trimMoreThanOneSpace(value);
                if (value == "" || value == null)
                {
                    addError(field, "<p>" + field.ToUpper() + " is Required</p>");
                }

            }
        }
        private void exact(Dictionary<string, int> exactFields)
        {
            foreach (KeyValuePair<string, int> field in exactFields)
            {

                string value = this.Form[field.Key];
                value = tools.trimMoreThanOneSpace(value);
                if (value.Length != field.Value)
                {
                    addError(field.Key, "<p>" + field.Key.ToUpper() + " length must be " + field.Value + "</p>");
                }
            }
        }
        private void range(Dictionary<string, List<int>> rangeLength)
        {
            foreach (KeyValuePair<string, List<int>> field in rangeLength)
            {

                string value = this.Form[field.Key];
                value = tools.trimMoreThanOneSpace(value);
                if (value.Length < field.Value[0] || value.Length > field.Value[1])
                    addError(field.Key, "<p>" + field.Key.ToUpper() + " mustn't be neither less than " + field.Value[0] + " characters nor more than " + field.Value[0] + " characters in length</p>");
            }
        }
        public Dictionary<string, List<string>> validateLogin()
        {
            this.errors = new Dictionary<string, List<string>>();
            try
            {
                int id = int.Parse(this.Form["id"]);
            }
            catch (Exception e)
            {
                addError("id", "<p>Id not valid</p>");
            }

            if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["password"])))
                addError("password", "<p>Login Password not valid</p>");
            return this.errors;

        }

        public static bool usernameExisits(string username)
        {
            extreemtEntities db = new extreemtEntities();
            return (db.users.Where(u => u.username == username).Count() > 0);
        }
        public static bool mailExisits(string mail)
        {
            extreemtEntities db = new extreemtEntities();
            return (db.users.Where(u => u.mail == mail).Count() > 0);
        }
        public string mode = "insert";
        public bool passwordChanged = false;
        public Dictionary<string, List<string>> validateSignUp()
        {
            this.errors = new Dictionary<string, List<string>>();

            if (!Account.isUserLogged())
            {
                addError("user", "<p>user Not Logged</p>");
                return this.errors;
            }
            if (this.mode == "insert")
            {
                string content = this.Form["a"];

                extreemt.crypt algo = new extreemt.crypt();
                string decryptedId = algo.Decrypt(content);
                string[] user = decryptedId.Split(new char[] { '$', '$', '$' });
                int i = 0;
                while (user[i] == "")
                {
                    i++;
                }
                if (user[i] != "2" && user[i] != "3")
                {

                    addError("user", "<p>error</p>");
                    return this.errors;
                }
                i++;
                while (user[i] == "")
                {
                    i++;
                }
                if (user[i] != "left" && user[i] != "right")
                {
                    addError("user", "<p>error</p>");
                    return this.errors;
                }
                i++;
                while (user[i] == "")
                {
                    i++;
                }
                string parentIdd = user[i];





                int parentId = int.Parse(parentIdd);

                extreemtEntities db = new extreemtEntities();
                if (db.users.Where(o => o.userId == parentId).Count() <= 0)
                {
                    addError("parentId", "<p>parent don't exist<p>");
                    return this.errors;
                }

            }
            user loggedUser = Account.staticGetLoggedUser();

            if (this.mode == "update")
            {
                if (this.Form["mail"] != loggedUser.mail)
                {
                    if (validateAccount.mailExisits(this.Form["mail"]))
                    {
                        addError("mail", "<p>Mail Exists - please choose another one</p>");
                        return this.errors;

                    }
                }
                if (this.Form["username"] != loggedUser.username)
                {
                    if (validateAccount.usernameExisits(this.Form["username"]))
                    {
                        addError("username", "<p>username Exists - please choose another one</p>");
                        return this.errors;
                    }
                }
            }
            if (this.mode == "insert")
            {
                if (validateAccount.mailExisits(this.Form["mail"]))
                {
                    addError("mail", "<p>Mail Exists - please choose another one</p>");
                    return this.errors;

                }



                if (validateAccount.usernameExisits(this.Form["username"]))
                {
                    addError("username", "<p>username Exists - please choose another one</p>");
                    return this.errors;
                }
            }
            //existance 
            List<string> requiredFields = new List<string> { "mail", "title", "username", "mobile"
            ,"country","city" , "address" ,"ssn" ,"nationality" 
               
            };
            bool Passwordchanged = false;
            if (this.mode == "insert")
            {
                requiredFields.Add("loginPassword");
                requiredFields.Add("binCode");
                requiredFields.Add("confirmBinCode");
                requiredFields.Add("confirmLoginPassword");
            }else if (this.mode=="update"){
                
                if (this.Form["loginPassword"] != null && this.Form["loginPassword"] != "")
                {
                    requiredFields.Add("newLoginPassword");
                    requiredFields.Add("confirmLoginPassword");
                    if (loggedUser.loginPassword == this.Form["loginPassword"])
                    {
                        if (this.Form["newLoginPassword"] == this.Form["confirmLoginPassword"]){
                            Passwordchanged = true;
                            
                        }
                        else
                        {
                            this.addError("newLoginPassword", "New Login And Confirm Login Dosn't Match ");
                        }

                    }
                    else
                    {
                        this.addError("loginPassword", "Pleasse Enter your old password");
                    }
                }
            }
            this.required(requiredFields);
            //length
            ////exact
            Dictionary<string, int> exactLength = new Dictionary<string, int>();
            exactLength.Add("ssn", 14);
            exactLength.Add("mobile", 11);
            if (this.Form["homephone"] != "" && this.Form["homephone"] != null)
            {
                exactLength.Add("homephone", 8);
            }
            exact(exactLength);

            ////range
            Dictionary<string, List<int>> rangeLength = new Dictionary<string, List<int>>();
            rangeLength.Add("mail", new List<int> { 5, 30 });
            rangeLength.Add("username", new List<int> { 3, 30 });

            rangeLength.Add("city", new List<int> { 3, 30 });
            rangeLength.Add("address", new List<int> { 3, 100 });
            rangeLength.Add("nationality", new List<int> { 3, 30 });
            if ((this.mode == "update" && Passwordchanged)||this.mode=="insert")
            {
                rangeLength.Add("loginPassword", new List<int> { 5, 30 });
            }
            
            if(this.mode=="insert")
            rangeLength.Add("binCode", new List<int> { 5, 30 });
            range(rangeLength);

            //password

            if (this.mode == "insert")
            {
                if (tools.trimMoreThanOneSpace(this.Form["loginPassword"]) != tools.trimMoreThanOneSpace(this.Form["confirmLoginPassword"]))
                    addError("confirmLoginPassword", "<p>Login password and confirm login password aren't exact</p>");
            
                if (tools.trimMoreThanOneSpace(this.Form["binCode"]) != tools.trimMoreThanOneSpace(this.Form["confirmBinCode"]))
                    addError("confirmBinCode", "<p>Bin Code and confirm Bin code aren't exact</p>");
            }

            //inclusion

            //pattern
            if (!validation.IsValidEmail(tools.trimMoreThanOneSpace(this.Form["mail"])))
                addError("mail", "<p>E-mail not valid</p>");
            if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["username"])))
                addError("username", "<p>username not valid</p>");

            if (!validation.isNumeric(tools.trimMoreThanOneSpace(this.Form["mobile"])))
                addError("mobile", "<p>Mobile not valid</p>");

            if (this.Form["homephone"] != "" && this.Form["homephone"] != null)
            {
                if (!validation.isNumeric(tools.trimMoreThanOneSpace(this.Form["homephone"])))
                    addError("homephone", "<p>Home Phone not valid</p>");
            }

            if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["city"])))
                addError("city", "<p>City not valid</p>");
            if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["address"])))
                addError("address", "<p>Address not valid</p>");
            if (!validation.isNumeric(tools.trimMoreThanOneSpace(this.Form["ssn"])))
                addError("ssn", "<p>SSN(ID Number) not valid</p>");
            if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["nationality"])))
                addError("nationality", "<p>Nationality not valid</p>");


            if (this.mode == "insert")
            {
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["loginPassword"])))
                    addError("loginPassword", "<p>Login Password not valid</p>");
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["confirmLoginPassword"])))
                    addError("confirmLoginPassword", "<p>Confirm Login Password not valid</p>");
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["binCode"])))
                    addError("binCode", "<p>Bin Code not valid</p>");
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["confirmBinCode"])))
                    addError("confirmBinCode", "<p>Confirm Bin Code not valid</p>");
            }
            else if (this.mode=="update" && Passwordchanged) {
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["loginPassword"])))
                    addError("loginPassword", "<p>Login Password not valid</p>");
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["newLoginPassword"])))
                    addError("newLoginPassword", "<p>new Login Password not valid</p>");
                if (!validation.isAlphaNumeric(tools.trimMoreThanOneSpace(this.Form["confirmLoginPassword"])))
                    addError("confirmLoginPassword", "<p>Confirm Login Password not valid</p>");
               
            }
            if (this.mode == "update")
            {
                this.passwordChanged = Passwordchanged;
            }

            return this.errors;
        }
    }
}