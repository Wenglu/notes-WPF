using System.Data;
using System.Data.SqlClient;

namespace Notatki.services
{
    public class DbConnectionFactory(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
