using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Products;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class HeaderType : ObjectGraphType<HeaderModel>
    {
        public HeaderType(CategoryService categoryService)
        {
            Name = "Header";

            Field<ListGraphType<SectionType>>(nameof(HeaderModel.SectionList),
                resolve: context =>
                {
                    return context.Source.SectionList;
                });
        }
    }
}