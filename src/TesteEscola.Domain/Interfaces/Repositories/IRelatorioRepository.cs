using System.Collections.Generic;
using TesteEscola.Domain.Dtos;

namespace TesteEscola.Domain.Interfaces.Repositories
{
    public interface IRelatorioRepository
    {
        IReadOnlyList<AlunosPorTurmaResponse> AlunosPorTurma();
    }
}
