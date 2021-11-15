using GraphQL;
using GraphQL.Types;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.ViewModels.Media;
using Litium.GraphQL.Models;
using Litium.GraphQL.Queries;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Administration.FieldFramework;
using Litium.Websites;
using System.Drawing;
using System.Globalization;

namespace Litium.GraphQL.Types
{
    [Service]
    public class CategoryType : ObjectGraphType<CategoryModel>
    {
        public CategoryType(CategoryService categoryService, BaseProductService baseProductService, 
            UrlService urlService, WebsiteService websiteService, RouteInfoService routeInfoService)
        {
            Name = "Category";
            Description = "A category";
            Field(p => p.SystemId, type: typeof(IdGraphType)).Description("The system Id");
            // Field(p => p.ParentSystemId, type: typeof(IdGraphType)).Description("The parent category system Id");
            Field(p => p.Slug).Description("The category's slug url")
                .Argument<GlobalInputType>("global")
                .Resolve(context =>
                {
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    var category = categoryService.Get(context.Source.SystemId);
                    return urlService.GetUrl(category, new CategoryUrlArgs(globalModel.ChannelSystemId));
                });

            Field<StringGraphType>(nameof(CategoryModel.Name), "The category name",
                arguments: new QueryArguments(
                     new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" }
                ),
                resolve: context =>
                {
                    var category = categoryService.Get(context.Source.SystemId);
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    var culture = CultureInfo.GetCultureInfo(globalModel.CurrentUICulture);
                    return category.GetEntityName(true, culture) ?? category.Id ?? "general.NameIsMissing".AsAngularResourceString();
                });

            Field<ListGraphType<StringGraphType>>(nameof(CategoryModel.Images), "The category's images",
                resolve: context =>
                {
                    var source = categoryService.Get(context.Source.SystemId);
                    return source.Fields.GetImageUrls((i) => {
                        return new ImageSize() { MinSize = new Size(200, 200), MaxSize = new Size(400, 400) };
                    });
                });

            FieldAsync<PageInfoProductType>(nameof(CategoryModel.Products), description: "Products belong to the category",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "offset" },
                    new QueryArgument<IntGraphType> { Name = "take" },
                    new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" }
                ),
                resolve: async context =>
                {
                    return await ProductQuery.SearchAsync(
                        context.GetArgument<GlobalModel>("global"),
                        context.Source.SystemId,
                        routeInfoService,
                        string.Empty,
                        context.GetArgument("take", 8),
                        context.GetArgument("offset", 0),
                        context.RequestServices
                    );
                });

            Field<PageInfoCategoryType>(nameof(CategoryModel.Categories), description: "Sub categories",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "offset" },
                    new QueryArgument<IntGraphType> { Name = "take" }
                ),
                resolve: context =>
                            new PageInfoModel<Category, CategoryModel>(categoryService.GetChildCategories(context.Source.SystemId), 
                                context.GetArgument("offset", 0), context.GetArgument("take", 10)));

            // Field<CategoryType>(nameof(CategoryModel.ParentCategory), description: "Parent category",
            //     resolve: context =>
            //                 categoryService.Get(context.Source.ParentSystemId)?.MapTo<CategoryModel>());
        }
    }

    [Service]
    public class PageInfoCategoryType : PageInfoTypeBase<CategoryType, Category, CategoryModel> { }
}