using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class LinkGraphType : ObjectGraphType<LinkModel>
    {
        public LinkGraphType()
        {
            Name = "Link";
            Field(p => p.Href, nullable: true);
            Field(p => p.Text);
        }
    }
}