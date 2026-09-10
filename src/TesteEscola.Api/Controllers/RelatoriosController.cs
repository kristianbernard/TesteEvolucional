using System.Web.Http;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Api.Controllers
{
    [RoutePrefix("api/relatorios")]
    public class RelatoriosController : ApiController
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        /// <summary>Por turma: nome, alunos matriculados e vagas restantes (consulta em SQL).</summary>
        [HttpGet, Route("alunos-por-turma")]
        public IHttpActionResult AlunosPorTurma()
        {
            return Ok(_relatorioService.AlunosPorTurma());
        }
    }
}
