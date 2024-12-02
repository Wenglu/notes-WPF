using Dapper;
using System;
using System.Data.SqlClient;

namespace Notatki.services
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string CreateDatabaseAndTablesIfNotExist()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string databaseScript = @"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Notatki')
                    BEGIN
                        CREATE DATABASE Notatki;
                    END;";
                connection.Execute(databaseScript);

                string userScript = @"
                    USE Notatki;

                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'UserAccounts')
                    BEGIN
                        CREATE TABLE UserAccounts (
                            Id INT PRIMARY KEY IDENTITY,
                            Username NVARCHAR(100),
                            Password NVARCHAR(MAX)
                        );
                    END;
                ";
                connection.Execute(userScript);

                string insertUserScript = @"
                    IF NOT EXISTS(
                        SELECT 1 FROM [UserAccounts] WHERE Username = @Login AND Password = @Haslo
                    )
                    BEGIN
                        INSERT INTO [UserAccounts] (Username, Password)
                        VALUES (@Login, @Haslo);
                    END
                    ";

                connection.Execute(insertUserScript, new { Login = "1", Haslo = "1" });

                string tableScript = @"
                    USE Notatki;

                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Note')
                    BEGIN
                        CREATE TABLE Note (
                            Id INT PRIMARY KEY IDENTITY,
                            Title NVARCHAR(100),
                            Content NVARCHAR(MAX),
                            Category NVARCHAR(50),
                            CreationDate DATETIME,
                            ModificationDate DATETIME
                        );
                    END;
                ";
                connection.Execute(tableScript);

                return $"Server={Environment.MachineName};Database=Notatki;Integrated Security=True;";
            }
        }
    }
}
