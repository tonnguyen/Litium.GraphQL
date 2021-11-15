using System;
using System.Linq;
using System.Threading.Tasks;
using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Search;
using Litium.GraphQL.Models;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Websites;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.GraphQL.Queries 
{
    internal class PageQuery
    {
        public static async Task<PageInfoModel<Page, PageModel>> SearchAsync(
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
            searchQuery.PageSize = take;
            searchQuery.PageNumber = (int)Math.Floor((double)offset / take) + 1;
            searchQuery.WebsiteSystemId = globalModel.WebsiteSystemId;
            // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
            // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
            using var scope = serviceProvider.CreateScope();
            var pageSearchService = scope.ServiceProvider.GetRequiredService<PageSearchService>();
            var searchResults = await pageSearchService.SearchAsync(searchQuery);
            if (searchResults == null)
            {
                return new PageInfoModel<Page, PageModel>()
                {
                    List = Enumerable.Empty<PageModel>(),
                    HasNextPage = false,
                    Total = 0
                };
            }
            // var pageItemBuilder = scope.ServiceProvider.GetRequiredService<PageItemViewModelBuilder>();
            return new PageInfoModel<Page, PageModel>()
            {
                List = searchResults.Items.Value.Cast<PageSearchResult>()
                    .Select(c => c.Item)
                    .MapEnumerableTo<PageModel>().ToList(),
                Total = searchResults.Total,
                HasNextPage = searchQuery.PageNumber * searchQuery.PageSize < searchResults.Total
            };
        }
    }
}