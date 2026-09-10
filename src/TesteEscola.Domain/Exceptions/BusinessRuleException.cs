namespace TesteEscola.Domain.Exceptions
{
    /// <summary>
    /// Uma regra de negócio impediu a operação (ex.: turma sem vaga, aluno
    /// inativo, matrícula duplicada). Traduzida para HTTP 409 (Conflict).
    /// </summary>
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
