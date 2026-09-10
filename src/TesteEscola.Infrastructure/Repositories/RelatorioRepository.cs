using System.Collections.Generic;
using System.Linq;
using Dapper;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Interfaces;
using TesteEscola.Domain.Interfaces.Repositories;

namespace TesteEscola.Infrastructure.Repositories
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly IDbConnectionFactory _factory;

        public RelatorioRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public IReadOnlyList<AlunosPorTurmaResponse> AlunosPorTurma()
        {
            const string sql = @"
SELECT  t.Id               AS TurmaId,
        t.Nome             AS Turma,
        COUNT(m.Id)        AS AlunosMatriculados,
        t.VagasDisponiveis AS VagasRestantes
FROM        dbo.Turma     t
LEFT JOIN   dbo.Matricula m ON m.TurmaId = t.Id
GROUP BY    t.Id, t.Nome, t.VagasDisponiveis
ORDER BY    t.Nome;";

            using (var conn = _factory.CreateConnection())
            {
                return conn.Query<AlunosPorTurmaResponse>(sql).ToList();
            }
        }
    }
}
