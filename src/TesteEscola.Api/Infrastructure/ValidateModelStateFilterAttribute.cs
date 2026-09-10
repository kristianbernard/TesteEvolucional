using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TesteEscola.Api.Infrastructure
{
    /// <summary>
    /// Se o ModelState estiver inválido (JSON malformado, campos obrigatórios
    /// ausentes, tipos incompatíveis), curto-circuita com 400 antes da action.
    /// </summary>
    public class ValidateModelStateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid)
                return;

            var erros = actionContext.ModelState
                .SelectMany(kv => kv.Value.Errors
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) && e.Exception != null
                        ? e.Exception.Message
                        : e.ErrorMessage))
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct()
                .ToList();

            if (erros.Count == 0)
                erros = new List<string> { "Requisição inválida." };

            actionContext.Response = actionContext.Request.CreateResponse(
                HttpStatusCode.BadRequest,
                new ApiError("Requisição inválida.", erros));
        }
    }
}
