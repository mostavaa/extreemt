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
            permissions.Add("user", new List<string> { "/Home/genology", 
                "/Home", 
                "/Home/Index", 
                "/Account/transfer",
                "/Account",
                "/Account/Index",
                "/Account/SignUp" ,
                "/Account/adminpage" 
            
            });
            permissions.Add("visitor", new List<string> { "/Home/Index", "/Account/Login" });

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