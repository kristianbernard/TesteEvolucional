using TesteEscola.Domain.Dtos;

namespace TesteEscola.Domain.Interfaces.Repositories
{
    public interface IMatriculaRepository
    {
        bool Exists(int alunoId, int turmaId);

        MatriculaResponse CriarComTransacao(int alunoId, int turmaId);
    }
}
