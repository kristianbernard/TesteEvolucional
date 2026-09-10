using System;
using System.Collections.Generic;
using System.Linq;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Entities;
using TesteEscola.Domain.Exceptions;
using TesteEscola.Domain.Interfaces.Repositories;
using TesteEscola.Domain.Interfaces.Services;
using EmailAddressAttribute = System.ComponentModel.DataAnnotations.EmailAddressAttribute;

namespace TesteEscola.Services
{
    public class AlunoService : IAlunoService
    {
        private static readonly DateTime DataNascimentoMinima = new DateTime(1900, 1, 1);
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public PagedResult<AlunoResponse> Listar(AlunosQuery query)
        {
            if (query == null) query = new AlunosQuery();

            var pagina = PaginacaoDefaults.NormalizarPagina(query.Pagina);
            var tamanho = PaginacaoDefaults.NormalizarTamanho(query.TamanhoPagina);

            int total;
            var alunos = _alunoRepository.Search(query.Nome, pagina, tamanho, query.IncluirInativos, out total);

            var itens = alunos.Select(Map).ToList();
            return new PagedResult<AlunoResponse>(itens, total, pagina, tamanho);
        }

        public AlunoResponse ObterPorId(int id)
        {
            var aluno = _alunoRepository.GetById(id);
            if (aluno == null)
                throw NotFoundException.For("Aluno", id);

            return Map(aluno);
        }

        public AlunoResponse Criar(CriarAlunoRequest request)
        {
            var erros = ValidarDados(request?.Nome, request?.Email, request?.DataNascimento);
            if (erros.Count > 0)
                throw new ValidationException(erros);

            var aluno = new Aluno
            {
                Nome = request.Nome.Trim(),
                Email = request.Email.Trim(),
                DataNascimento = request.DataNascimento.Value.Date,
                Ativo = true
            };

            var novoId = _alunoRepository.Insert(aluno);
            return Map(_alunoRepository.GetById(novoId));
        }

        public AlunoResponse Atualizar(int id, AtualizarAlunoRequest request)
        {
            var erros = ValidarDados(request?.Nome, request?.Email, request?.DataNascimento);
            if (request?.Ativo == null)
                erros.Add("Ativo é obrigatório.");
            if (erros.Count > 0)
                throw new ValidationException(erros);

            var aluno = _alunoRepository.GetById(id);
            if (aluno == null)
                throw NotFoundException.For("Aluno", id);

            aluno.Nome = request.Nome.Trim();
            aluno.Email = request.Email.Trim();
            aluno.DataNascimento = request.DataNascimento.Value.Date;
            aluno.Ativo = request.Ativo.Value;

            _alunoRepository.Update(aluno);
            return Map(_alunoRepository.GetById(id));
        }

        public void Remover(int id)
        {
            if (!_alunoRepository.SoftDelete(id))
                throw NotFoundException.For("Aluno", id);
        }

        private static List<string> ValidarDados(string nome, string email, DateTime? dataNascimento)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(nome))
                erros.Add("Nome é obrigatório.");
            else if (nome.Trim().Length > 120)
                erros.Add("Nome deve ter no máximo 120 caracteres.");

            if (string.IsNullOrWhiteSpace(email))
                erros.Add("Email é obrigatório.");
            else if (email.Trim().Length > 120 || !new EmailAddressAttribute().IsValid(email.Trim()))
                erros.Add("Email inválido.");

            if (dataNascimento == null)
                erros.Add("DataNascimento é obrigatória.");
            else if (dataNascimento.Value.Date >= DateTime.Today)
                erros.Add("DataNascimento deve ser uma data no passado.");
            else if (dataNascimento.Value.Date < DataNascimentoMinima)
                erros.Add("DataNascimento inválida.");

            return erros;
        }

        private static AlunoResponse Map(Aluno a)
        {
            if (a == null) return null;
            return new AlunoResponse
            {
                Id = a.Id,
                Nome = a.Nome,
                Email = a.Email,
                DataNascimento = a.DataNascimento,
                Ativo = a.Ativo,
                DataCadastro = a.DataCadastro
            };
        }
    }
}
