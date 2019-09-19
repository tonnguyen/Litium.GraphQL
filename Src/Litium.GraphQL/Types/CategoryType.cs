using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Products;
using Litium.Runtime.DependencyInjection;
using System.Linq;

namespace Litium.GraphQL.Types
{
    [Service]
    public class CategoryType : ObjectGraphType<CategoryModel>
    {
        public CategoryType(CategoryService categoryService, BaseProductService baseProductService)
        {
            Name = "Category";
            Description = "A category";
            Field(p => p.SystemId, type: typeof(IdGraphType)).Description("The system Id");
            Field(p => p.Name).Description("The category name");
            Field(p => p.Images).Description("The category image list");
            Field<PageInfoProductType>(nameof(CategoryModel.Products), description: "Products belong to the category",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "skip" },
                    new QueryArgument<IntGraphType> { Name = "take" }
                ),
                resolve: context =>
                {
                    var productLinks = categoryService.Get(context.Source.SystemId).ProductLinks;
                    return new PageInfoModel<BaseProduct, ProductModel>(productLinks.Select(p => baseProductService.Get(p.BaseProductSystemId)),
                                context.GetArgument("skip", 0), context.GetArgument("take", 10));
                });
            Field<PageInfoCategoryType>(nameof(CategoryModel.Categories), description: "Sub categories",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "skip" },
                    new QueryArgument<IntGraphType> { Name = "take" }
                ),
                resolve: context =>
                            new PageInfoModel<Category, CategoryModel>(categoryService.GetChildCategories(context.Source.SystemId), 
                                context.GetArgument("skip", 0), context.GetArgument("take", 10)));
        }
    }

    [Service]
    public class PageInfoCategoryType : PageInfoTypeBase<CategoryType, Category, CategoryModel> { }
}