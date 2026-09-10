using System.Web.Http;
using TesteEscola.Api.App_Start;

namespace TesteEscola.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(DependencyInjectionConfig.Register);
            GlobalConfiguration.Configure(SwaggerConfig.Register);
        }
    }
}
