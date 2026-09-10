using System.Collections.Generic;
using TesteEscola.Domain.Entities;

namespace TesteEscola.Domain.Interfaces.Repositories
{
    public interface IAlunoRepository
    {
        Aluno GetById(int id);

        IReadOnlyList<Aluno> Search(string nome, int pagina, int tamanhoPagina, bool incluirInativos, out int total);

        int Insert(Aluno aluno);

        bool Update(Aluno aluno);

        bool SoftDelete(int id);
    }
}
