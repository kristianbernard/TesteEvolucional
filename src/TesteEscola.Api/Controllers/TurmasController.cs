using System.Web.Http;
using TesteEscola.Domain.Interfaces.Services;

namespace TesteEscola.Api.Controllers
{
    [RoutePrefix("api/turmas")]
    public class TurmasController : ApiController
    {
        private readonly ITurmaService _turmaService;

        public TurmasController(ITurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        /// <summary>Lista as turmas com a quantidade de vagas restantes.</summary>
        [HttpGet, Route("")]
        public IHttpActionResult Listar()
        {
            return Ok(_turmaService.Listar());
        }
    }
}
