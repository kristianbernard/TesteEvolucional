using TesteEscola.Domain.Dtos;

namespace TesteEscola.Domain.Interfaces.Services
{
    public interface IAlunoService
    {
        PagedResult<AlunoResponse> Listar(AlunosQuery query);
        AlunoResponse ObterPorId(int id);
        AlunoResponse Criar(CriarAlunoRequest request);
        AlunoResponse Atualizar(int id, AtualizarAlunoRequest request);
        void Remover(int id);
    }
}
