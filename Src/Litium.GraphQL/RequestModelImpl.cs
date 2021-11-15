using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Globalization;
using Litium.Sales;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Products;
using Litium.Web.Models.Websites;
using System;

namespace Litium.GraphQL
{
    public class RequestModelImpl : RequestModel
    {
        public Lazy<ChannelModel> _channelModel;
        public Lazy<SearchQuery> _searchQuery;
        public Lazy<PageModel> _currentPageModel;
        public Lazy<ProductModel> _currentProductModel;
        public Lazy<CategoryModel> _currentCategoryModel;

        public RequestModelImpl(
            CartContext cartContext,
            CountryService countryService)
            : base(
                  cartContext,
                  countryService)
        {
        }

        public override ChannelModel ChannelModel => _channelModel?.Value;
        public override SearchQuery SearchQuery => _searchQuery?.Value;
        public override PageModel CurrentPageModel => _currentPageModel?.Value;
        public override ProductModel CurrentProductModel => _currentProductModel?.Value;
        public override CategoryModel CurrentCategoryModel => _currentCategoryModel?.Value;
    }
}
