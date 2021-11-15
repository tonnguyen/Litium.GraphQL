using GraphQL.Types;
using Litium.GraphQL.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Litium.GraphQL
{
    public class LitiumSchema : Schema
    {
        public LitiumSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<LitiumQuery>();
        }
    }
}
