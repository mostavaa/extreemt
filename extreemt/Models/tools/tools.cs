using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace extreemt
{
    public class tools
    {
        public static Dictionary<string,List<string>> permissions = new Dictionary<string,List<string>>();
        public static string removeSpaces(string input)
        {
            input = trimMoreThanOneSpace(input);
            return input.Replace(" ", "");

        }
        private static void fillPermissions()
        {
            permissions = new Dictionary<string, List<string>>();
            permissions.Add("user", new List<string> { 
                "/Home/genology", 
                "/Home", 
                "/Home/Index", 
                "/Home/Profile" ,
                "/Account/transfer",
                "/Account/transferMoney",
                "/Account/SignUp" ,
                "/Account/adminpage" ,
                "/Account/taskscheduledforcommmission" ,
                "/Account/Sumbit_edit" ,
                "/Account/submitSignUp" ,

                "/Account/logout" ,
                "/Account/getUserByIdForTransfer" ,

                "/Product" ,
                "/Product/Index" ,
                "/Product/Details" ,
                "/Product/selectUser" ,
                "/Product/Buy" ,
                "/Product/activateUser" ,
                "/Product/updateActiveParents" ,
                "/Registered/Cash_Bank" ,
                "/Registered/Product_Bank" ,
                "/Registered/Account_Summary" ,
                "/Registered/Group_list" ,
                "/Registered/Daily_Signup" ,
                "/Registered/My_Transfer" ,
                "/Registered/My_Basket" ,
                "/Registered/My_Order" ,
                "/Registered/Update_Info" ,
            });
            permissions.Add("admin", new List<string> { 
                "/Home/genology", 
                "/Home", 
                "/Home/Index", 
                "/Home/Profile" ,
                "/Account/transfer",
                "/Account/transferMoney",
                "/Account/SignUp" ,
                "/Account/adminpage" ,
                "/Account/taskscheduledforcommmission" ,
                "/Account/Sumbit_edit" ,
                "/Account/submitSignUp" ,

                "/Account/logout" ,
                "/Account/getUserByIdForTransfer" ,


                "/Product/selectUser" ,
                "/Product/Buy" ,
                "/Product/activateUser" ,
                "/Product/updateActiveParents" ,
                "/Registered/Cash_Bank" ,
                "/Registered/Product_Bank" ,
                "/Registered/Account_Summary" ,
                "/Registered/Group_list" ,
                "/Registered/Daily_Signup" ,
                "/Registered/My_Transfer" ,
                "/Registered/My_Basket" ,
                "/Registered/My_Order" ,
                "/Registered/Update_Info" ,
       
            
                                "/Product" ,
                "/Product/Index" ,
                "/Product/Details" ,
                "/Product/Create" ,
                "/Product/insertProductPhotos" ,
                "/Product/deleteProductPhotos" ,
                "/Product/DeleteAllPhotosEdit" ,
                "/Product/Edit" ,
                "/Product/Delete" ,

                                "/Category" ,
                "/Category/Index" ,
                "/Category/Details" ,
                "/Category/Create" ,
                "/Category/Edit" ,
                "/Category/Delete" ,
                "/Category/DeleteConfirmed" ,

                                "/Account/Admin" ,
                "/Account/changeAdminCredit" ,
                "/Account/getAdminCredit" ,
                "/Account/changeSignUp" ,
                "/Account/getSignUpStatus" ,
            });
            permissions.Add("visitor", new List<string> { 
                "/Home/Index",
                "/Account/Login",
                "/Account/taskscheduledforcommmission" ,
                "/Account/submitLogin" ,


            });

        }
        public static bool isAuthorized(string page)
        {
            tools.fillPermissions();
            string current = "visitor";
            if (HttpContext.Current.Session["userId"] == null)
            {
                current = "visitor";
            }
            else
            {
                int userId = int.Parse(HttpContext.Current.Session["userId"].ToString());
                if (userId != 0)
                {
                    current = "user";
                    extreemtEntities db = new extreemtEntities();
                    if (db.users.Where(u => u.userId == userId).Count() > 0)
                    {
                        user admin = db.users.Where(u => u.userId == userId).First();
                        if (Account.isAdmin(admin))
                        {
                            current = "admin";
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, List<string>> item in tools.permissions)
            {
                if (item.Key == current)
                {
                    foreach (string pg in item.Value)
                    {
                        if (page.Contains(pg))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return tools.permissions[current].Contains(page);
        }
        public static string trimMoreThanOneSpace(string input)
        {
            input = input.Trim();
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            return regex.Replace(input, @" ");
        }
        public static string generateRandomNumber(int length = 8)
        {
            string characters = "0123456789";
            int charactersLength = characters.Length;
            string randomString = "";
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                randomString += characters[rand.Next(0, charactersLength - 1)];
            }
            return randomString;
        }



    }
}