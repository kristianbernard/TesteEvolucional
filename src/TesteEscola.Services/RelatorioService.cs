using System.Collections.Generic;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Interfaces.Repositories;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IRelatorioRepository _relatorioRepository;

        public RelatorioService(IRelatorioRepository relatorioRepository)
        {
            _relatorioRepository = relatorioRepository;
        }

        public IReadOnlyList<AlunosPorTurmaResponse> AlunosPorTurma()
        {
            return _relatorioRepository.AlunosPorTurma();
        }
    }
}
