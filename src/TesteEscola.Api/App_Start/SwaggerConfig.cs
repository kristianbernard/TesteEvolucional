using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Swashbuckle.Application;

namespace TesteEscola.Api.App_Start
{
    /// <summary>
    /// Configura o Swagger (Swashbuckle para ASP.NET Web API clássico).
    /// UI em /swagger; documento OpenAPI em /swagger/docs/v1.
    /// </summary>
    public static class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "TesteEscola API")
                     .Description("API de controle de matrículas de uma escola (teste prático .NET Pleno).");

                    // Usa os comentários XML dos controllers/DTOs se o arquivo existir.
                    var xmlPath = HostingEnvironment.MapPath("~/bin/TesteEscola.Api.xml");
                    if (xmlPath != null && File.Exists(xmlPath))
                        c.IncludeXmlComments(xmlPath);

                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("TesteEscola API");
                });
        }
    }
}
