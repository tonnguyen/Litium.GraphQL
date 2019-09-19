
using Microsoft.Owin;

namespace Litium.GraphQL
{
    internal class GraphiQLConfig
    {
        internal GraphQLOptions Options { get; }

        private GraphiQLConfig(GraphQLOptions options)
        {
            Options = options;
        }

        internal static GraphQLOptions Bootstrap(GraphQLOptions options)
        {
            return new GraphiQLConfig(options).Options;
        }
    }

    internal class GraphQLOptions
    {
        internal PathString Path { get; set; }
    }
}