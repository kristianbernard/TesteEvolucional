namespace TesteEscola.Domain.Exceptions
{
    /// <summary>Recurso não encontrado. Traduzida para HTTP 404.</summary>
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message) { }

        public static NotFoundException For(string recurso, int id)
        {
            return new NotFoundException(recurso + " " + id + " não encontrado(a).");
        }
    }
}
