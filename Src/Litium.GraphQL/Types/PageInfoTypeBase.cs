using GraphQL.Types;
using Litium.GraphQL.Models;

namespace Litium.GraphQL.Types
{
    public abstract class PageInfoTypeBase<TObjectGraphType, TModel, TViewModel> : ObjectGraphType<PageInfoModel<TModel, TViewModel>>
        where TModel : class
        where TObjectGraphType : ObjectGraphType<TViewModel>
    {
        public PageInfoTypeBase()
        {
            Field<ListGraphType<TObjectGraphType>>(
                "list",
                resolve: context => context.Source.List
            );
            Field(x => x.Total);
            Field(x => x.HasNextPage);
        }
    }
}
