using System;
using System.Reflection;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;

namespace Litium.GraphQL
{
    /// <summary>
    /// Require handler to system.webServer
    /// <add name="Owin" verb="" path="*" type="Microsoft.Owin.Host.SystemWeb.OwinHttpHandler, Microsoft.Owin.Host.SystemWeb"/>
    /// </summary>
    public static class GraphiQLExtensions
    {
        public static IAppBuilder UseGraphiQl(this IAppBuilder app, string path = "/graphql")
        {
            return UseGraphiQl(app, path, GraphiQLConfig.Bootstrap);
        }

        private static IAppBuilder UseGraphiQl(this IAppBuilder app, string path, Func<GraphQLOptions, GraphQLOptions> config)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var assembly = typeof(GraphiQLExtensions).GetTypeInfo().Assembly;

            path = path.StartsWith("/") ? path : "/" + path;

            var graphQlOptions = new GraphQLOptions
            {
                Path = new PathString(path)
            };

            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                RequestPath = config(graphQlOptions).Path,
                FileSystem = new EmbeddedResourceFileSystem(assembly, "Litium.GraphQL.assets"),
                StaticFileOptions = { ContentTypeProvider = new FileExtensionContentTypeProvider() },
            };

            app.UseFileServer(options);

            return app;
        }
    }
}