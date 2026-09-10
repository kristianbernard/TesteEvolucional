using System.Collections.Generic;

namespace TesteEscola.Domain.Dtos
{
    public class PagedResult<T>
    {
        public PagedResult(IReadOnlyList<T> itens, int total, int pagina, int tamanhoPagina)
        {
            Itens = itens ?? new List<T>();
            Total = total;
            Pagina = pagina;
            TamanhoPagina = tamanhoPagina;
        }

        public IReadOnlyList<T> Itens { get; }
        public int Total { get; }
        public int Pagina { get; }
        public int TamanhoPagina { get; }
        public int TotalPaginas
        {
            get { return TamanhoPagina > 0 ? (Total + TamanhoPagina - 1) / TamanhoPagina : 0; }
        }
    }
}
