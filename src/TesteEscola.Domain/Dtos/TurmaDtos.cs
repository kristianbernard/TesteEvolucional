namespace TesteEscola.Domain.Dtos
{
    public class TurmaResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Periodo { get; set; }
        public int VagasTotal { get; set; }

        public int VagasRestantes { get; set; }
    }
}
