using System;
using Dapper;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Exceptions;
using TesteEscola.Domain.Interfaces;
using TesteEscola.Domain.Interfaces.Repositories;

namespace TesteEscola.Infrastructure.Repositories
{
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly IDbConnectionFactory _factory;

        public MatriculaRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public bool Exists(int alunoId, int turmaId)
        {
            const string sql =
                "SELECT COUNT(1) FROM dbo.Matricula WHERE AlunoId = @alunoId AND TurmaId = @turmaId;";
            using (var conn = _factory.CreateConnection())
            {
                return conn.ExecuteScalar<int>(sql, new { alunoId, turmaId }) > 0;
            }
        }

        public MatriculaResponse CriarComTransacao(int alunoId, int turmaId)
        {
            using (var conn = _factory.CreateConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        var vagas = conn.ExecuteScalar<int?>(
                            "SELECT VagasDisponiveis FROM dbo.Turma WITH (UPDLOCK, HOLDLOCK) WHERE Id = @turmaId;",
                            new { turmaId }, tx);

                        if (vagas == null)
                            throw NotFoundException.For("Turma", turmaId);

                        if (vagas.Value <= 0)
                            throw new BusinessRuleException("A turma não possui vagas disponíveis.");

                        var duplicada = conn.ExecuteScalar<int>(
                            "SELECT COUNT(1) FROM dbo.Matricula WHERE AlunoId = @alunoId AND TurmaId = @turmaId;",
                            new { alunoId, turmaId }, tx) > 0;

                        if (duplicada)
                            throw new BusinessRuleException("O aluno já está matriculado nesta turma.");

                        var dataMatricula = DateTime.Now;

                        var id = conn.ExecuteScalar<int>(
                            @"INSERT INTO dbo.Matricula (AlunoId, TurmaId, DataMatricula)
                              OUTPUT INSERTED.Id
                              VALUES (@alunoId, @turmaId, @dataMatricula);",
                            new { alunoId, turmaId, dataMatricula }, tx);

                        var afetadas = conn.Execute(
                            @"UPDATE dbo.Turma
                              SET VagasDisponiveis = VagasDisponiveis - 1
                              WHERE Id = @turmaId AND VagasDisponiveis > 0;",
                            new { turmaId }, tx);

                        if (afetadas != 1)
                            throw new BusinessRuleException("A turma não possui vagas disponíveis.");

                        tx.Commit();

                        return new MatriculaResponse
                        {
                            Id = id,
                            AlunoId = alunoId,
                            TurmaId = turmaId,
                            DataMatricula = dataMatricula
                        };
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
