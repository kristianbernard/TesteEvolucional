using System;
using System.Net;
using System.Web.Http;
using TesteEscola.Api.Infrastructure;
using TesteEscola.Domain.Dtos;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Api.Controllers
{
    [RoutePrefix("api/matriculas")]
    public class MatriculasController : ApiController
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculasController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        /// <summary>
        /// Matricula um aluno numa turma. Regras (turma com vaga, aluno ativo,
        /// sem matrícula duplicada) retornam 409; aluno/turma inexistentes, 404.
        /// </summary>
        [HttpPost, Route("")]
        public IHttpActionResult Criar([FromBody] CriarMatriculaRequest request)
        {
            if (request == null)
                return Content(HttpStatusCode.BadRequest, new ApiError("Corpo da requisição ausente."));

            var matricula = _matriculaService.Criar(request);
            return Created(new Uri(Request.RequestUri, "/api/matriculas/" + matricula.Id), matricula);
        }
    }
}
