namespace TesteEscola.Domain.Dtos
{
    public class AlunosPorTurmaResponse
    {
        public int TurmaId { get; set; }
        public string Turma { get; set; }
        public int AlunosMatriculados { get; set; }
        public int VagasRestantes { get; set; }
    }
}
