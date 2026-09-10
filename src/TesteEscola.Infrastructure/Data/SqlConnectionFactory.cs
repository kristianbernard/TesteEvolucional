using System;
using System.Data;
using System.Data.SqlClient;
using TesteEscola.Domain.Interfaces;

namespace TesteEscola.Infrastructure.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string não configurada.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
