using System;
using System.Linq;
using System.Threading.Tasks;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Search;
using Litium.GraphQL.Models;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.GraphQL.Queries 
{
    internal class ProductQuery
    {
        public static async Task<PageInfoModel<BaseProduct, ProductModel>> SearchAsync(
            GlobalModel globalModel,
            Guid? parentCategorySystemId,
            RouteInfoService routeInfoService,
            string query,
            int take,
            int offset,
            IServiceProvider serviceProvider
        )
        {
            routeInfoService.Setup(globalModel, null);
            var searchQuery = new SearchQuery();
            searchQuery.CategorySystemId = parentCategorySystemId;
            searchQuery.CategoryShowRecursively = true;
            searchQuery.ProductListSystemId = null;
            searchQuery.Text = query ?? string.Empty;
            searchQuery.PageSize = take;
            searchQuery.PageNumber = (int)Math.Floor((double)offset / take) + 1;
            searchQuery.WebsiteSystemId = globalModel.WebsiteSystemId;
            // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
            // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
            using var scope = serviceProvider.CreateScope();
            var productSearchService = scope.ServiceProvider.GetRequiredService<ProductSearchService>();
            var searchResults = await productSearchService.SearchAsync(searchQuery, searchQuery.Tags, true, true, true);
            if (searchResults == null)
            {
                return new PageInfoModel<BaseProduct, ProductModel>()
                {
                    List = Enumerable.Empty<ProductModel>(),
                    HasNextPage = false,
                    Total = 0
                };
            }
            var productItemBuilder = scope.ServiceProvider.GetRequiredService<ProductItemViewModelBuilder>();
            return new PageInfoModel<BaseProduct, ProductModel>()
            {
                List = searchResults.Items.Value.Cast<ProductSearchResult>()
                    .Select(c => productItemBuilder.Build(c.Item))
                    .MapEnumerableTo<ProductModel>().ToList(),
                Total = searchResults.Total,
                HasNextPage = searchQuery.PageNumber * searchQuery.PageSize < searchResults.Total
            };
        }
    }
}