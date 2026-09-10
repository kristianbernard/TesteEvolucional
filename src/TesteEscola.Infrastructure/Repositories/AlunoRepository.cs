using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TesteEscola.Domain.Entities;
using TesteEscola.Domain.Interfaces;
using TesteEscola.Domain.Interfaces.Repositories;

namespace TesteEscola.Infrastructure.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly IDbConnectionFactory _factory;

        public AlunoRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        private const string ColunasAluno =
            "Id, Nome, Email, DataNascimento, Ativo, DataCadastro";

        public Aluno GetById(int id)
        {
            const string sql = "SELECT " + ColunasAluno + " FROM dbo.Aluno WHERE Id = @id;";
            using (var conn = _factory.CreateConnection())
            {
                return conn.QuerySingleOrDefault<Aluno>(sql, new { id });
            }
        }

        public IReadOnlyList<Aluno> Search(string nome, int pagina, int tamanhoPagina, bool incluirInativos, out int total)
        {
            const string sql = @"
SELECT COUNT(1)
FROM dbo.Aluno
WHERE (@nome IS NULL OR Nome LIKE @nome)
  AND (@incluirInativos = 1 OR Ativo = 1);

SELECT " + ColunasAluno + @"
FROM dbo.Aluno
WHERE (@nome IS NULL OR Nome LIKE @nome)
  AND (@incluirInativos = 1 OR Ativo = 1)
ORDER BY Nome
OFFSET @offset ROWS FETCH NEXT @take ROWS ONLY;";

            var parametros = new
            {
                nome = string.IsNullOrWhiteSpace(nome) ? null : "%" + nome.Trim() + "%",
                incluirInativos = incluirInativos ? 1 : 0,
                offset = (pagina - 1) * tamanhoPagina,
                take = tamanhoPagina
            };

            using (var conn = _factory.CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parametros))
            {
                total = multi.ReadSingle<int>();
                return multi.Read<Aluno>().ToList();
            }
        }

        public int Insert(Aluno aluno)
        {
            const string sql = @"
INSERT INTO dbo.Aluno (Nome, Email, DataNascimento, Ativo)
OUTPUT INSERTED.Id
VALUES (@Nome, @Email, @DataNascimento, @Ativo);";

            using (var conn = _factory.CreateConnection())
            {
                return conn.ExecuteScalar<int>(sql, new
                {
                    aluno.Nome,
                    aluno.Email,
                    aluno.DataNascimento,
                    aluno.Ativo
                });
            }
        }

        public bool Update(Aluno aluno)
        {
            const string sql = @"
UPDATE dbo.Aluno
SET Nome = @Nome,
    Email = @Email,
    DataNascimento = @DataNascimento,
    Ativo = @Ativo
WHERE Id = @Id;";

            using (var conn = _factory.CreateConnection())
            {
                var linhas = conn.Execute(sql, new
                {
                    aluno.Id,
                    aluno.Nome,
                    aluno.Email,
                    aluno.DataNascimento,
                    aluno.Ativo
                });
                return linhas > 0;
            }
        }

        public bool SoftDelete(int id)
        {
            const string sql = "UPDATE dbo.Aluno SET Ativo = 0 WHERE Id = @id;";
            using (var conn = _factory.CreateConnection())
            {
                return conn.Execute(sql, new { id }) > 0;
            }
        }
    }
}
