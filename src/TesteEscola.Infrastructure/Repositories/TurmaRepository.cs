using System.Collections.Generic;
using System.Linq;
using Dapper;
using TesteEscola.Domain.Entities;
using TesteEscola.Domain.Interfaces;
using TesteEscola.Domain.Interfaces.Repositories;

namespace TesteEscola.Infrastructure.Repositories
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly IDbConnectionFactory _factory;

        public TurmaRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        private const string ColunasTurma =
            "Id, Nome, Periodo, VagasTotal, VagasDisponiveis";

        public IReadOnlyList<Turma> GetAll()
        {
            const string sql = "SELECT " + ColunasTurma + " FROM dbo.Turma ORDER BY Nome;";
            using (var conn = _factory.CreateConnection())
            {
                return conn.Query<Turma>(sql).ToList();
            }
        }

        public Turma GetById(int id)
        {
            const string sql = "SELECT " + ColunasTurma + " FROM dbo.Turma WHERE Id = @id;";
            using (var conn = _factory.CreateConnection())
            {
                return conn.QuerySingleOrDefault<Turma>(sql, new { id });
            }
        }
    }
}
