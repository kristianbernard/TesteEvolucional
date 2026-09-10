using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using TesteEscola.Domain.Exceptions;

namespace TesteEscola.Api.Infrastructure
{
    /// <summary>
    /// Converte exceções previsíveis de domínio no status HTTP correto:
    /// 400 (validação), 404 (não encontrado), 409 (regra de negócio).
    /// Qualquer outra exceção segue o fluxo padrão (500).
    /// </summary>
    public class DomainExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;
            HttpResponseMessage resposta = null;

            var validation = ex as ValidationException;
            if (validation != null)
            {
                resposta = context.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ApiError("Requisição inválida.", validation.Errors));
            }

            var notFound = ex as NotFoundException;
            if (notFound != null)
            {
                resposta = context.Request.CreateResponse(
                    HttpStatusCode.NotFound,
                    new ApiError(notFound.Message));
            }

            var businessRule = ex as BusinessRuleException;
            if (businessRule != null)
            {
                resposta = context.Request.CreateResponse(
                    HttpStatusCode.Conflict,
                    new ApiError(businessRule.Message));
            }

            if (resposta != null)
                context.Response = resposta;
        }
    }
}
