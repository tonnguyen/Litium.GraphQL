﻿    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Litium.Accelerator.Constants;
    using Litium.Accelerator.Routing;
    using Litium.Accelerator.ViewModels.Framework;
    using Litium.Accelerator.ViewModels.Search;
    using Litium.FieldFramework.FieldTypes;
    using Litium.Globalization;
    using Litium.Runtime.AutoMapper;
    using Litium.Web.Models;
    using Litium.Accelerator.Services;
    using Litium.Sales;
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    namespace Litium.Accelerator.Builders.Framework
    {
    public class ClientContextViewModelBuilder : IViewModelBuilder<ClientContextViewModel>
    {
        private readonly SiteSettingViewModelBuilder _siteSettingViewModelBuilder;
        private readonly CartViewModelBuilder _cartViewModelBuilder;
        private readonly NavigationViewModelBuilder _navigationViewModelBuilder;
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly CartContextAccessor _cartContextAccessor;

        public ClientContextViewModelBuilder(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            RequestModelAccessor requestModelAccessor,
            CartContextAccessor cartContextAccessor,
            SiteSettingViewModelBuilder siteSettingViewModelBuilder,
            CartViewModelBuilder cartViewModelBuilder,
            NavigationViewModelBuilder navigationViewModelBuilder)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
            _requestModelAccessor = requestModelAccessor;
            _cartContextAccessor = cartContextAccessor;
            _siteSettingViewModelBuilder = siteSettingViewModelBuilder;
            _cartViewModelBuilder = cartViewModelBuilder;
            _navigationViewModelBuilder = navigationViewModelBuilder;
        }

        public async Task<ClientContextViewModel> BuildAsync()
        {
            var tokens = _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext);
            var requestVerification = tokens.RequestToken;
            var cartContext = _cartContextAccessor.CartContext; 

            return new ClientContextViewModel()
            {
                SiteSetting = _siteSettingViewModelBuilder.Build(),
                Cart = _cartViewModelBuilder.Build(cartContext),
                Navigation = await _navigationViewModelBuilder.BuildAsync(),
                Countries = _requestModelAccessor.RequestModel.ChannelModel.Channel.CountryLinks
                            .Select(link => link.CountrySystemId.MapTo<Country>())
                            .Select(country => new ListItem(new RegionInfo(country.Id).DisplayName, country.Id))
                            .OrderBy(country => country.Text)
                            .ToList(),
                RequestVerificationToken = requestVerification,
                QuickSearchUrl = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.SearchResultPage)?.MapTo<LinkModel>()?.Href ?? "",
                Texts = GetClientTexts()
            };
        }

        private IDictionary<string, string> GetClientTexts()
        {
            var webSite = _requestModelAccessor.RequestModel.WebsiteModel;
            if (webSite == null)
            {
                return new Dictionary<string, string>();
            }
            return webSite.Website.Texts.GetTextContainer(CultureInfo.CurrentUICulture).Where(t => t.Key.StartsWith("js.")).ToDictionary(t => t.Key.Replace("js.", string.Empty), t => t.Value);
        }
    }
    }
