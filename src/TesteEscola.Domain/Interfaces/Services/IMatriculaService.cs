using TesteEscola.Domain.Dtos;

namespace TesteEscola.Domain.Interfaces.Services
{
    public interface IMatriculaService
    {
        MatriculaResponse Criar(CriarMatriculaRequest request);
    }
}
