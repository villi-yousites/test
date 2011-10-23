using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Security;

namespace Presentation.Models
{
    public class UserModel
    {
        private readonly string connStr;
        public string Username { get; set; }
        public string Password { get; set; }

        public UserModel(string dbName = "villi_test")
        {
            connStr =
                "Data Source=GAL-PC\\SQLEXPRESS;Initial Catalog=villi_test;Persist Security Info=True;User ID=villi;Password=vk123;User Instance=False";
        }

        public bool Login(string user, string pass)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "Select * from Users WHERE Username='" + user + "'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            if (reader["Password"].ToString() == pass)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}