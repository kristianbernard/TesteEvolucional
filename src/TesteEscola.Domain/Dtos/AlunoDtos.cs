using System;
using System.ComponentModel.DataAnnotations;

namespace TesteEscola.Domain.Dtos
{
    public class AlunoResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class AlunosQuery
    {
        public string Nome { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }

        public bool IncluirInativos { get; set; }
    }

    public class CriarAlunoRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nome é obrigatório.")]
        [StringLength(120, ErrorMessage = "Nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email é obrigatório.")]
        [StringLength(120, ErrorMessage = "Email deve ter no máximo 120 caracteres.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "DataNascimento é obrigatória.")]
        public DateTime? DataNascimento { get; set; }
    }

    public class AtualizarAlunoRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nome é obrigatório.")]
        [StringLength(120, ErrorMessage = "Nome deve ter no máximo 120 caracteres.")]
        public string Nome { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email é obrigatório.")]
        [StringLength(120, ErrorMessage = "Email deve ter no máximo 120 caracteres.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "DataNascimento é obrigatória.")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "Ativo é obrigatório.")]
        public bool? Ativo { get; set; }
    }
}
