using Dapper;
using Notatki;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Notatki.Model;

namespace Notatki.services
{
    using System.Data.SqlClient;
    using System.Linq;
    using Dapper;

    class Authorize
    {
        public bool loginUser(string username, string password)
        {
            var connectionString = $"Server={Environment.MachineName};Integrated Security=True;";



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var selectQuery = "SELECT * FROM UserAccounts;";
                var userList = connection.Query<User>(selectQuery);
                var userLinq = from user in userList
                               where user.Username == username && user.Password == password
                               select true;


                return userLinq.FirstOrDefault(false);
            }
        }
    }

}
