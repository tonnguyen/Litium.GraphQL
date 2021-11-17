using GraphQL.Types;
using Litium.Application.Web.Routing;
using Litium.Data;
using Litium.GraphQL.Models;
using Litium.GraphQL.Types;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using System;
using System.Globalization;
using GraphQL;
using System.Linq;
using Litium.Globalization;
using Litium.Websites;
using Litium.Accelerator.Constants;
using Microsoft.Extensions.DependencyInjection;
using Litium.Accelerator.Builders.Product;

namespace Litium.GraphQL.Queries
{
    [Service()]
    public class LitiumQuery : ObjectGraphType<object>
    {
        public LitiumQuery(BaseProductService baseProductService,
            VariantService variantService,
            ChannelService channelService,
            MarketService marketService,
            CountryService countryService,
            LanguageService languageService,
            RoutingHelperService routingHelperService,
            CategoryService categoryService,
            PageService pageService,
            WebsiteService websiteService,
            RouteInfoService routeInfoService,
            DataService dataService)
        {
            Name = "LitiumQuery";

            Field<WebsiteType>(
                "website",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "systemId" }
                ),
                resolve: (context) =>
                {
                    var systemId = context.GetArgument<Guid>("systemId");
                    var logoUrl = websiteService.Get(systemId).Fields
                                .GetValue<Guid?>(AcceleratorWebsiteFieldNameConstants.LogotypeMain).Value
                                .MapTo<Web.Models.ImageModel>().GetUrlToImage(System.Drawing.Size.Empty, new System.Drawing.Size(-1, 50)).Url;
                    return new WebsiteModel()
                    {
                        LogoUrl = logoUrl,
                        SystemId = systemId
                    };
                }
            );

            Field<GlobalType>(
                "global",
                resolve: (context) =>
                {
                    var channel = channelService.GetAll().First();
                    var market = marketService.Get(channel.MarketSystemId.Value);
                    var country = countryService.Get(channel.CountryLinks.First().CountrySystemId);
                    var culture = languageService.Get(channel.WebsiteLanguageSystemId.Value);
                    var uiCulture = languageService.Get(channel.ProductLanguageSystemId.Value);

                    return new GlobalModel()
                    {
                        ChannelSystemId = channel.SystemId,
                        WebsiteSystemId = channel.WebsiteSystemId.Value,
                        AssortmentSystemId = market.AssortmentSystemId.Value,
                        CountrySystemId = country.SystemId,
                        CurrencySystemId = country.CurrencySystemId,
                        CurrentCulture = culture.Id,
                        CurrentUICulture = uiCulture.Id,
                    };
                }
            );

            Field<CategoryType>(
                "category",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "systemId", Description = "id of the category, will be override if slug has value" },
                    new QueryArgument<GlobalInputType> { Name = "global" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "slug", Description = "category slug urls" }
                ),
                resolve: context =>
                    {
                        var categoryId = context.GetArgument<Guid>("systemId");
                        var slug = context.GetArgument<string[]>("slug");
                        if (slug != null && slug.Any())
                        {
                            var globalModel = context.GetArgument<GlobalModel>("global");
                            return GetCategoryFromSlug(slug, globalModel, routingHelperService, categoryService);
                        }
                        return categoryService.Get(categoryId).MapTo<CategoryModel>();
                    }
                );

            FieldAsync<PageInfoCategoryType>(
                "searchCategory",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "offset" },
                    new QueryArgument<IntGraphType> { Name = "take" },
                    new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" },
                    new QueryArgument<StringGraphType> { Name = "query", Description = "The search query" }
                ),
                resolve: async context =>
                {
                    return await CategoryQuery.SearchAsync(
                        context.GetArgument<GlobalModel>("global"),
                        routeInfoService,
                        context.GetArgument<string>("query") ?? string.Empty,
                        context.GetArgument("take", 8),
                        context.GetArgument("offset", 0),
                        context.RequestServices
                    );
                }
            );

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "systemId", Description = "id of the product" },
                    new QueryArgument<StringGraphType> { Name = "slug", Description = "Product's slug url" },
                    new QueryArgument<GlobalInputType> { Name = "global" }
                ),
                resolve: context => 
                {
                    var systemId = context.GetArgument<Guid>("systemId");
                    var slug = context.GetArgument<string>("slug");
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    if (!string.IsNullOrEmpty(slug)) {
                        systemId = GetProductSystemIdFromSlug(slug, globalModel, routingHelperService);
                    }
                    // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
                    // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
                    using var scope = context.RequestServices.CreateScope();
                    return GetProduct(systemId, scope, baseProductService, variantService, globalModel);
                }
                );

            FieldAsync<PageInfoProductType>(
                "products",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "offset" },
                    new QueryArgument<IntGraphType> { Name = "take" },
                    new QueryArgument<IdGraphType> { Name = "parentCategorySystemId", Description = "The optional parent category system id" },
                    new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" },
                    new QueryArgument<StringGraphType> { Name = "query", Description = "The search query" }
                ),
                resolve: async context =>
                {
                    return await ProductQuery.SearchAsync(
                        context.GetArgument<GlobalModel>("global"),
                        context.GetArgument<Guid?>("parentCategorySystemId"),
                        routeInfoService,
                        context.GetArgument<string>("query") ?? string.Empty,
                        context.GetArgument("take", 8),
                        context.GetArgument("offset", 0),
                        context.RequestServices
                    );
                }
            );

            Field<PageType>(
                "page",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "systemId", Description = "Id of the page. Can be empty GUID, the website's homepage will be returned in that case, where WebsiteSystemId should have value." },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "slug", Description = "Page's slugs, will override systemId if available" },
                    new QueryArgument<GlobalInputType> { Name = "global", Description = "Global context, which is needed to get page from slug" }
                ),
                resolve: context => 
                {
                    var slug = context.GetArgument<string[]>("slug");
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    routeInfoService.Setup(globalModel, null);
                    if (slug != null && slug.Any())
                    {
                        return GetPageFromSlug(slug, globalModel, pageService, routingHelperService);
                    }

                    var systemId = context.GetArgument<Guid?>("systemId");
                    if (systemId.HasValue && systemId.Value != Guid.Empty)
                    {
                        return pageService.Get(systemId.Value)?.MapTo<PageModel>();
                    }
                    return pageService.GetChildPages(Guid.Empty, globalModel.WebsiteSystemId).FirstOrDefault()?.MapTo<PageModel>();
                }
            );

            FieldAsync<PageInfoPageType>(
                "searchPage",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "offset" },
                    new QueryArgument<IntGraphType> { Name = "take" },
                    new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" },
                    new QueryArgument<StringGraphType> { Name = "query", Description = "The search query" }
                ),
                resolve: async context =>
                {
                    return await PageQuery.SearchAsync(
                        context.GetArgument<GlobalModel>("global"),
                        routeInfoService,
                        context.GetArgument<string>("query") ?? string.Empty,
                        context.GetArgument("take", 8),
                        context.GetArgument("offset", 0),
                        context.RequestServices
                    );
                }
            );

            Field<ContentType>(
                "content",
                arguments: new QueryArguments(
                    new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "slug", Description = "The slug array" }
                ),
                resolve: context =>
                {
                    var slug = context.GetArgument<string[]>("slug");
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    routeInfoService.Setup(globalModel, null);

                    var systemId = GetProductSystemIdFromSlug(slug[slug.Length - 1], globalModel, routingHelperService);
                    if (systemId != Guid.Empty)
                    {
                        // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
                        // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
                        using var scope = context.RequestServices.CreateScope();
                        return GetProduct(systemId, scope, baseProductService, variantService, globalModel);
                    }

                    var category = GetCategoryFromSlug(slug, globalModel, routingHelperService, categoryService);
                    if (category != null)
                    {
                        return category;
                    }
                    return GetPageFromSlug(slug, globalModel, pageService, routingHelperService);
                }
            );
        }

        private static Guid GetProductSystemIdFromSlug(string slug, GlobalModel globalModel, RoutingHelperService routingHelperService)
        {
            var systemId = Guid.Empty;
            var culture = CultureInfo.GetCultureInfo(globalModel.CurrentUICulture);
            if (!routingHelperService.TryGetBaseProduct(slug, culture, out systemId)) {
                routingHelperService.TryGetVariant(slug, culture, out systemId);
            }
            return systemId;
        }

        private static ProductModel GetProduct(Guid systemId, IServiceScope scope, 
            BaseProductService baseProductService, VariantService variantService,
            GlobalModel globalModel)
        {
            // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
            // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
            var productPageViewModelBuilder = scope.ServiceProvider.GetRequiredService<ProductPageViewModelBuilder>();
            var baseProduct = baseProductService.Get(systemId);
            if (baseProduct != null)
            {
                return productPageViewModelBuilder.Build(baseProduct).MapTo<ProductModel>();
            }
            return productPageViewModelBuilder.Build(variantService.Get(systemId)).MapTo<ProductModel>();
        }

        private static CategoryModel GetCategoryFromSlug(string[] slug, GlobalModel globalModel, 
            RoutingHelperService routingHelperService, CategoryService categoryService)
        {
            var culture = CultureInfo.GetCultureInfo(globalModel.CurrentUICulture);
            var categoryId = Guid.Empty;
            foreach (var segment in slug)
            {
                routingHelperService.TryGetCategory(categoryId, 
                    segment, culture, out categoryId, globalModel.AssortmentSystemId);
            }
            if (categoryId == Guid.Empty)
            {
                return null;
            }
            return categoryService.Get(categoryId).MapTo<CategoryModel>();
        }

        private static PageModel GetPageFromSlug(string[] slug, GlobalModel globalModel, PageService pageService,
            RoutingHelperService routingHelperService)
        {
            var culture = CultureInfo.GetCultureInfo(globalModel.CurrentCulture);
            var startPage = pageService.GetChildPages(Guid.Empty, globalModel.WebsiteSystemId).FirstOrDefault();
            var pageId = startPage.SystemId;
            foreach (var segment in slug)
            {
                routingHelperService.TryGetPage(pageId, segment, culture, out pageId, globalModel.WebsiteSystemId);
            }
            return pageService.Get(pageId).MapTo<PageModel>();
        }
    }
}