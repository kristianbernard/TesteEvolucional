using System.Collections.Generic;
using TesteEscola.Domain.Dtos;

namespace TesteEscola.Domain.Interfaces.Services
{
    public interface IRelatorioService
    {
        IReadOnlyList<AlunosPorTurmaResponse> AlunosPorTurma();
    }
}
