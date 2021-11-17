using GraphQL.Types;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class ContentType : UnionGraphType
    {
        public ContentType()
        {
            Name = "Content";
            Type<PageType>();
            Type<CategoryType>();
            Type<ProductType>();
        }
    }
}