using System.Net;
using System.Net.Http;
using System.Web.Http;
using TesteEscola.Api.Infrastructure;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Api.Controllers
{
    [RoutePrefix("api/alunos")]
    public class AlunosController : ApiController
    {
        private readonly IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpGet, Route("")]
        public IHttpActionResult Listar(
            [FromUri] string nome = null,
            [FromUri] int pagina = 1,
            [FromUri] int tamanhoPagina = 10,
            [FromUri] bool incluirInativos = false)
        {
            var resultado = _alunoService.Listar(new AlunosQuery
            {
                Nome = nome,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                IncluirInativos = incluirInativos
            });

            // Total no corpo (resultado.Total)
            var resposta = Request.CreateResponse(HttpStatusCode.OK, resultado);
            resposta.Headers.Add("X-Total-Count", resultado.Total.ToString());
            return ResponseMessage(resposta);
        }

        /// <summary>Obtém um aluno por Id.</summary>
        [HttpGet, Route("{id:int}")]
        public IHttpActionResult ObterPorId(int id)
        {
            return Ok(_alunoService.ObterPorId(id));
        }

        /// <summary>Cria um aluno.</summary>
        [HttpPost, Route("")]
        public IHttpActionResult Criar([FromBody] CriarAlunoRequest request)
        {
            if (request == null)
                return Content(HttpStatusCode.BadRequest, new ApiError("Corpo da requisição ausente."));

            var criado = _alunoService.Criar(request);
            return Created(new System.Uri(Request.RequestUri, "/api/alunos/" + criado.Id), criado);
        }

        /// <summary>Atualiza um aluno.</summary>
        [HttpPut, Route("{id:int}")]
        public IHttpActionResult Atualizar(int id, [FromBody] AtualizarAlunoRequest request)
        {
            if (request == null)
                return Content(HttpStatusCode.BadRequest, new ApiError("Corpo da requisição ausente."));

            return Ok(_alunoService.Atualizar(id, request));
        }

        /// <summary>Exclusão lógica (seta Ativo = 0).</summary>
        [HttpDelete, Route("{id:int}")]
        public IHttpActionResult Remover(int id)
        {
            _alunoService.Remover(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
