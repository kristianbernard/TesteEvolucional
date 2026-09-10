using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using TesteEscola.Domain.Interfaces;
using TesteEscola.Domain.Interfaces.Repositories;
using TesteEscola.Domain.Interfaces.Services;
using TesteEscola.Infrastructure.Data;
using TesteEscola.Infrastructure.Repositories;
using TesteEscola.Services;

namespace TesteEscola.Api.App_Start
{
    public static class DependencyInjectionConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            var connectionString = ConfigurationManager.ConnectionStrings["TesteEscola"];
            if (connectionString == null || string.IsNullOrWhiteSpace(connectionString.ConnectionString))
                throw new ConfigurationErrorsException(
                    "Connection string 'TesteEscola' não encontrada no Web.config.");

            var cs = connectionString.ConnectionString;

            builder.Register(c => new SqlConnectionFactory(cs))
                   .As<IDbConnectionFactory>()
                   .SingleInstance();

            builder.RegisterType<AlunoRepository>().As<IAlunoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TurmaRepository>().As<ITurmaRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MatriculaRepository>().As<IMatriculaRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RelatorioRepository>().As<IRelatorioRepository>().InstancePerLifetimeScope();

            builder.RegisterType<AlunoService>().As<IAlunoService>().InstancePerLifetimeScope();
            builder.RegisterType<TurmaService>().As<ITurmaService>().InstancePerLifetimeScope();
            builder.RegisterType<MatriculaService>().As<IMatriculaService>().InstancePerLifetimeScope();
            builder.RegisterType<RelatorioService>().As<IRelatorioService>().InstancePerLifetimeScope();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
