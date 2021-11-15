using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Globalization;
using Litium.GraphQL.Models;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Websites;
using Litium.Web.Routing;
using Litium.Websites;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;

namespace Litium.GraphQL
{
    [Service]
    public class RouteInfoService
    {
        private readonly ChannelService _channelService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly RouteRequestLookupInfoAccessor _routeRequestLookupInfoAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CountryService _countryService;
        private readonly PageService _pageService;
        private readonly PageModelBuilder _pageModelBuilder;

        public RouteInfoService(RequestModelAccessor requestModelAccessor, 
            RouteRequestLookupInfoAccessor routeRequestLookupInfoAccessor,
            IHttpContextAccessor httpContextAccessor, CountryService countryService,
            ChannelService channelService, PageService pageService, PageModelBuilder pageModelBuilder)
        {
            _channelService = channelService;
            _requestModelAccessor = requestModelAccessor;
            _routeRequestLookupInfoAccessor = routeRequestLookupInfoAccessor;
            _httpContextAccessor = httpContextAccessor;
            _countryService = countryService;
            _pageService = pageService;
            _pageModelBuilder = pageModelBuilder;
        }

        public void Setup(GlobalModel globalModel, Guid? pageSystemId)
        {
            var channel = _channelService.Get(globalModel.ChannelSystemId);
            _requestModelAccessor.RequestModel = new RequestModelImpl(_httpContextAccessor.HttpContext.GetCartContext(),
                _countryService)
            {
                _channelModel = new Lazy<ChannelModel>(() => channel.MapTo<ChannelModel>()),
                _searchQuery = new Lazy<SearchQuery>(() => default), //filterContext.HttpContext.MapTo<SearchQuery>()),
                _currentPageModel = new Lazy<Web.Models.Websites.PageModel>(() => pageSystemId.HasValue && pageSystemId.Value != Guid.Empty ? 
                                        new Web.Models.Websites.PageModel(_pageService.Get(pageSystemId.Value))
                                        : new Web.Models.Websites.PageModel(_pageService.GetChildPages(Guid.Empty, globalModel.WebsiteSystemId).FirstOrDefault())),
                _currentCategoryModel = new Lazy<Web.Models.Products.CategoryModel>(() => default), //(_routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData)?.CategorySystemId.MapTo<CategoryModel>()),
                _currentProductModel = new Lazy<Web.Models.Products.ProductModel>(() => default),
                //=> (_routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData)?.VariantSystemId.MapTo<Variant>()?.MapTo<ProductModel>()
                //?? (_routeRequestInfoAccessor.RouteRequestInfo?.Data as ProductPageData)?.BaseProductSystemId.MapTo<BaseProduct>()?.MapTo<ProductModel>()),
            };
            _routeRequestLookupInfoAccessor.RouteRequestLookupInfo = new RouteRequestLookupInfo
            {
                IsInAdministration = false,
                Channel = channel,
            };
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(globalModel.CurrentCulture);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(globalModel.CurrentUICulture);
        }
    }
}
