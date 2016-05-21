using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ZuoraBillingAppUnitTest
{
    public static class AppSettings
    {
        public static string Username { 
            get {
                return ConfigurationManager.AppSettings["username"].ToString();
            } 
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["password"].ToString();
            }
        }
    }
}
