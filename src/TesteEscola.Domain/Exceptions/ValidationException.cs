using System.Collections.Generic;

namespace TesteEscola.Domain.Exceptions
{
    /// <summary>Requisição inválida (dados malformados). Traduzida para HTTP 400.</summary>
    public class ValidationException : DomainException
    {
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new[] { message };
        }

        public ValidationException(IReadOnlyList<string> errors)
            : base(errors != null && errors.Count > 0 ? string.Join(" ", errors) : "Requisição inválida.")
        {
            Errors = errors ?? new List<string>();
        }
    }
}
