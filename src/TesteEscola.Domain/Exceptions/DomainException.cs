using System;

namespace TesteEscola.Domain.Exceptions
{
    /// <summary>
    /// Base de todas as exceções previsíveis da aplicação. O filtro de exceções
    /// da Web API traduz cada subtipo no status HTTP correto, evitando 500 para
    /// erros de validação ou de regra de negócio.
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }
}
