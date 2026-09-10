using System.Collections.Generic;
using System.Linq;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Interfaces.Repositories;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Services
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _turmaRepository;

        public TurmaService(ITurmaRepository turmaRepository)
        {
            _turmaRepository = turmaRepository;
        }

        public IReadOnlyList<TurmaResponse> Listar()
        {
            return _turmaRepository.GetAll()
                .Select(t => new TurmaResponse
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Periodo = t.Periodo,
                    VagasTotal = t.VagasTotal,
                    VagasRestantes = t.VagasDisponiveis
                })
                .ToList();
        }
    }
}
