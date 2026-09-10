using System;
using System.ComponentModel.DataAnnotations;

namespace TesteEscola.Domain.Dtos
{
    public class CriarMatriculaRequest
    {
        [Required(ErrorMessage = "AlunoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "AlunoId inválido.")]
        public int? AlunoId { get; set; }

        [Required(ErrorMessage = "TurmaId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "TurmaId inválido.")]
        public int? TurmaId { get; set; }
    }

    public class MatriculaResponse
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
