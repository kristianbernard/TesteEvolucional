using System.Collections.Generic;
using System.Linq;

namespace TesteEscola.Api.Infrastructure
{
    /// <summary>Corpo padrão das respostas de erro (400/404/409).</summary>
    public class ApiError
    {
        public ApiError(string mensagem, IEnumerable<string> erros = null)
        {
            Mensagem = mensagem;
            Erros = (erros ?? Enumerable.Empty<string>()).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();
        }

        public string Mensagem { get; }
        public string[] Erros { get; }
    }
}
