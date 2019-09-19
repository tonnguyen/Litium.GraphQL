using GraphQL.Types;
using Litium.Data;
using Litium.GraphQL.Models;
using Litium.GraphQL.Types;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using System;

namespace Litium.GraphQL.Queries
{
    [Service()]
    public class LitiumQuery : ObjectGraphType<object>
    {
        public LitiumQuery(BaseProductService baseProductService, 
            CategoryService categoryService,
            DataService dataService)
        {
            Name = "LitiumQuery";

            Field<CategoryType>(
                "category",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "systemId", Description = "id of the category" }
                ),
                resolve: (context) => categoryService.Get(context.GetArgument<Guid>("systemId")).MapTo<CategoryModel>()
            );

            Field<PageInfoCategoryType>(
                "categories",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "assortmentSystemId", Description = "id of the assortment" },
                    new QueryArgument<StringGraphType> { Name = "systemId", Description = "id of the parent category" },
                    new QueryArgument<IntGraphType> { Name = "skip" },
                    new QueryArgument<IntGraphType> { Name = "take" }
                ),
                resolve: context =>
                    {
                        var assortmentId = context.GetArgument<Guid>("assortmentSystemId");
                        var parentCategoryId = context.GetArgument<Guid>("systemId");
                        var items = (assortmentId != null && assortmentId != Guid.Empty
                            ? categoryService.GetChildCategories(Guid.Empty, assortmentId)
                            : categoryService.GetChildCategories(parentCategoryId));
                        return new PageInfoModel<Category, CategoryModel>(items, context.GetArgument("skip", 0), context.GetArgument("take", 10));
                    }
                );

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "systemId", Description = "id of the product" }
                ),
                resolve: context => baseProductService.Get(context.GetArgument<Guid>("systemId")).MapTo<ProductModel>()
                );

            Field<PageInfoProductType>(
                "products",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "skip" },
                    new QueryArgument<IntGraphType> { Name = "take" }
                ),
                resolve: context =>
                {
                    using (var q = dataService.CreateQuery<BaseProduct>())
                    {
                        var total = q.Count();
                        var skip = context.GetArgument("skip", 0);
                        var take = context.GetArgument("take", 10);
                        var result = baseProductService.Get(q
                            .Skip(skip)
                            .Take(take)
                            .ToSystemIdList())
                            .MapEnumerableTo<ProductModel>();
                        return new PageInfoModel<BaseProduct, ProductModel>() { List = result, Total = total, HasNextPage = skip + take < total };
                    }
                }
            );
        }
    }
}