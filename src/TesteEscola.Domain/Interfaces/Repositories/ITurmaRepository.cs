using System.Collections.Generic;
using TesteEscola.Domain.Entities;

namespace TesteEscola.Domain.Interfaces.Repositories
{
    public interface ITurmaRepository
    {
        IReadOnlyList<Turma> GetAll();

        Turma GetById(int id);
    }
}
