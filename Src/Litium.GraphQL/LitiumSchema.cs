using GraphQL;
using GraphQL.Types;
using Litium.GraphQL.Queries;
using Litium.Runtime.DependencyInjection;
using System;

namespace Litium.GraphQL
{
    [Service(ServiceType = typeof(ISchema), Lifetime = DependencyLifetime.Singleton)]
    public class LitiumSchema : Schema
    {
        public LitiumSchema(LitiumQuery query, IServiceProvider serviceProvider)
            : base(new FuncDependencyResolver(type => serviceProvider.GetService(type)))
        {
            Query = query;
        }
    }
}
