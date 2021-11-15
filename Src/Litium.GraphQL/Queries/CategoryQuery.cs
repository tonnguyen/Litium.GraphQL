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
    internal class CategoryQuery
    {
        public static async Task<PageInfoModel<Category, CategoryModel>> SearchAsync(
            GlobalModel globalModel,
            RouteInfoService routeInfoService,
            string query,
            int take,
            int offset,
            IServiceProvider serviceProvider
        )
        {
            routeInfoService.Setup(globalModel, null);
            var searchQuery = new SearchQuery();
            searchQuery.ProductListSystemId = null;
            searchQuery.Text = query ?? string.Empty;
            // searchQuery.CategoryShowRecursively = categoryShowRecursively;
            searchQuery.PageSize = take;
            searchQuery.PageNumber = (int)Math.Floor((double)offset / take) + 1;
            searchQuery.WebsiteSystemId = globalModel.WebsiteSystemId;
            // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
            // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
            using var scope = serviceProvider.CreateScope();
            var categorySearchService = scope.ServiceProvider.GetRequiredService<CategorySearchService>();
            var searchResults = await categorySearchService.SearchAsync(searchQuery);
            if (searchResults == null)
            {
                return new PageInfoModel<Category, CategoryModel>()
                {
                    List = Enumerable.Empty<CategoryModel>(),
                    HasNextPage = false,
                    Total = 0
                };
            }
            var categoryItemBuilder = scope.ServiceProvider.GetRequiredService<CategoryItemViewModelBuilder>();
            return new PageInfoModel<Category, CategoryModel>()
            {
                List = searchResults.Items.Value.Cast<CategorySearchResult>()
                    .Select(c => categoryItemBuilder.Build(c.Id))
                    .MapEnumerableTo<CategoryModel>().ToList(),
                Total = searchResults.Total,
                HasNextPage = searchQuery.PageNumber * searchQuery.PageSize < searchResults.Total
            };
        }
    }
}