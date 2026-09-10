using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TesteEscola.Api.Infrastructure;

namespace TesteEscola.Api.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Somente rotas por atributo — cada controller declara suas rotas.
            config.MapHttpAttributeRoutes();

            // JSON como formato único (evita 406/negociação com XML).
            config.Formatters.Clear();
            var json = new JsonMediaTypeFormatter();
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            json.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            json.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.Add(json);

            // Traduz exceções de domínio em 400/404/409 (nunca 500 p/ validação).
            config.Filters.Add(new DomainExceptionFilterAttribute());

            // Modelo inválido (ModelState) => 400 padronizado, antes de entrar na action.
            config.Filters.Add(new ValidateModelStateFilterAttribute());
        }
    }
}
