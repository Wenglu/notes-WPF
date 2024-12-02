using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Notatki.services;

namespace Notatki.services
{
    public class UserManager
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UserManager(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public bool RegisterUser(string username, string password)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                // Check if the username already exists
                string userQuery = "SELECT Id FROM UserAccounts WHERE Username = @Username;";
                int? existingUserId = connection.QueryFirstOrDefault<int?>(userQuery, new { Username = username });
                if (existingUserId.HasValue)
                {
                    return false; // User already exists
                }

                // If the username doesn't exist, proceed with registration
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertQuery = "INSERT INTO UserAccounts (Username, Password) VALUES (@Username, @Password);";
                        connection.Execute(insertQuery, new { Username = username, Password = password }, transaction);
                        transaction.Commit();
                        return true; // Registration successful
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error registering user: {ex.Message}");
                        return false; // Registration failed
                    }
                }
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();


            string query = "SELECT Password FROM UserAccounts WHERE Username = @Username;";
            string passwordFromBase = connection.QueryFirstOrDefault<string>(query, new { Username = username });

            if (passwordFromBase == null)
            {
                return false;
            }

            return password == passwordFromBase;
        }
    }
}
