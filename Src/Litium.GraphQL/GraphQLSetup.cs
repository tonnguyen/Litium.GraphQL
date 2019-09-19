using GraphQL;
using GraphQL.Http;
using Litium.Owin.InversionOfControl;
using Litium.Owin.Lifecycle;
using Litium.Web.WebApi;
using Owin;
using System.Reflection;
using System.Web.Http;

namespace Litium.GraphQL
{
    internal class GraphQLSetup : IComponentInstaller, IOwinStartupConfiguration, IWebApiSetup
    {
        private static readonly string Route = "api";
        public static readonly string RoutePrefix = $"~/{Route}/";

        public void Install(IIoCContainer container, Assembly[] assemblies)
        {
            container.For<IDocumentExecuter>().UsingFactoryMethod(() => new DocumentExecuter()).RegisterAsSingleton();
            container.For<IDocumentWriter>().UsingFactoryMethod(() => new DocumentWriter()).RegisterAsSingleton();
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseGraphiQl("/graphql");
        }

        public void SetupWebApi(HttpConfiguration config)
        {
            config.EnableCors();
            // Convention-based routing            
            config.Routes.MapHttpRoute(
                name: "GraphQLApi",
                routeTemplate: Route + "/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}