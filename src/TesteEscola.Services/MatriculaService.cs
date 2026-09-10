using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Exceptions;
using TesteEscola.Domain.Interfaces.Repositories;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly ITurmaRepository _turmaRepository;
        private readonly IMatriculaRepository _matriculaRepository;

        public MatriculaService(
            IAlunoRepository alunoRepository,
            ITurmaRepository turmaRepository,
            IMatriculaRepository matriculaRepository)
        {
            _alunoRepository = alunoRepository;
            _turmaRepository = turmaRepository;
            _matriculaRepository = matriculaRepository;
        }

        public MatriculaResponse Criar(CriarMatriculaRequest request)
        {
            if (request == null || request.AlunoId == null || request.TurmaId == null)
                throw new ValidationException("AlunoId e TurmaId são obrigatórios.");

            var alunoId = request.AlunoId.Value;
            var turmaId = request.TurmaId.Value;

            var aluno = _alunoRepository.GetById(alunoId);
            if (aluno == null)
                throw NotFoundException.For("Aluno", alunoId);

            if (!aluno.Ativo)
                throw new BusinessRuleException("O aluno está inativo e não pode ser matriculado.");

            var turma = _turmaRepository.GetById(turmaId);
            if (turma == null)
                throw NotFoundException.For("Turma", turmaId);

            if (turma.VagasDisponiveis <= 0)
                throw new BusinessRuleException("A turma não possui vagas disponíveis.");

            if (_matriculaRepository.Exists(alunoId, turmaId))
                throw new BusinessRuleException("O aluno já está matriculado nesta turma.");

            return _matriculaRepository.CriarComTransacao(alunoId, turmaId);
        }
    }
}
