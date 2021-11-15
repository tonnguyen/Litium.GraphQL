using System;
using GraphQL;
using GraphQL.Caching;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.GraphQL
{
    [Service(ServiceType = typeof(IStartupFilter))]
    internal class Bootstrap : IStartupFilter, IApplicationConfiguration
    {
        public void Configure(ApplicationConfigurationBuilder app)
        {
            app.ConfigureServices(services =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("DefaultGraphQLCorsPolicy", builder =>
                    {
                        builder.AllowAnyHeader()
                            .WithMethods("GET", "POST")
                            .WithOrigins("http://localhost:3000", "https://localhost", "https://accspa");
                    });
                });

                services.AddSingleton<IDocumentCache>(services =>
                {
                    return new MemoryDocumentCache(new MemoryDocumentCacheOptions
                    {
                        // maximum total cached query length of 1,000,000 bytes (assume 10x memory usage
                        // for 10MB maximum memory use by the cache)
                        SizeLimit = 1000000,
                        // no expiration of cached queries (cached queries are only ejected when the cache is full)
                        SlidingExpiration = null,
                    });
                });

                services.AddSingleton<IDocumentExecuter>(services =>
                {
                    return new DocumentExecuter(
                        new GraphQLDocumentBuilder(),
                        new DocumentValidator(),
                        new ComplexityAnalyzer(),
                        services.GetRequiredService<IDocumentCache>());
                });

                services.AddSingleton<ISchema, LitiumSchema>();

                services.AddGraphQL(options =>
                {
                    options.EnableMetrics = true;
                })
                .AddErrorInfoProvider(opt => opt.ExposeExceptionStackTrace = true)
                .AddSystemTextJson()
                //.AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User })
                ;
            });
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                next(app);
                app.UseCors("DefaultGraphQLCorsPolicy");
                // add http for Schema at default url /graphql
                app.UseGraphQL<ISchema>();
                // use graphql-playground at default url /ui/playground
                app.UseGraphQLPlayground();
            };
        }
    }
}