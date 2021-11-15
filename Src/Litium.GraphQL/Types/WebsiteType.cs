using GraphQL;
using GraphQL.Types;
using Litium.Accelerator.Constants;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;
using Litium.Websites;
using System.Collections.Generic;
using System.Linq;
using Litium.FieldFramework;
using Litium.Runtime.AutoMapper;
using System.Globalization;
using Litium.FieldFramework.FieldTypes;
using System;
using Litium.Products;
using Litium.Web.Administration.FieldFramework;
using Litium.Web;

namespace Litium.GraphQL.Types
{
    [Service]
    public class WebsiteType : ObjectGraphType<WebsiteModel>
    {
        public WebsiteType(WebsiteService websiteService, 
            RouteInfoService routeInfoService, 
            CategoryService categoryService,
            UrlService urlService,
            PageService pageService)
        {
            Name = "Website";
            Field(p => p.LogoUrl);
            Field(p => p.SystemId, type: typeof(IdGraphType));
            
            Field<FooterType>(nameof(WebsiteModel.Footer), "The website footer",
                arguments: new QueryArguments(
                    new QueryArgument<GlobalInputType> { Name = "global" }
                ),
                resolve: context =>
                {
                    var website = websiteService.Get(context.Source.SystemId);
                    var footer = website.Fields.GetValue<IList<MultiFieldItem>>(AcceleratorWebsiteFieldNameConstants.Footer);
                    if (footer != null)
                    {
                        var globalModel = context.GetArgument<GlobalModel>("global");
                        var culture = CultureInfo.GetCultureInfo(globalModel.CurrentUICulture);
                        var startPageSystemId = pageService.GetChildPages(Guid.Empty, globalModel.WebsiteSystemId).FirstOrDefault()?.SystemId;
                        routeInfoService.Setup(globalModel, startPageSystemId);
                        return new FooterModel()
                        {
                            SectionList = footer.Select(c => new SectionModel()
                            {
                                SectionTitle = c.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterHeader, culture) ?? string.Empty,
                                SectionLinkList = c.Fields.GetValue<IList<PointerItem>>(AcceleratorWebsiteFieldNameConstants.FooterLinkList)
                                    .OfType<PointerPageItem>().ToList()
                                    .Select(x => x.MapTo<Web.Models.LinkModel>()).Where(c => c != null).Select(l => new Models.LinkModel()
                                    {
                                        Href = l.Href,
                                        Text = l.Text
                                    }).ToList() ?? new List<Models.LinkModel>(),
                                SectionText = c.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterText, culture) ?? string.Empty
                            }).ToList()
                        };
                    }
                    return null;
                });

            Field<HeaderType>(nameof(WebsiteModel.Header), "The website header",
                arguments: new QueryArguments(
                    new QueryArgument<GlobalInputType> { Name = "global" }
                ),
                resolve: context =>
                {
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    var items = categoryService.GetChildCategories(Guid.Empty, globalModel.AssortmentSystemId);
                    var culture = CultureInfo.GetCultureInfo(globalModel.CurrentCulture);
                    return new HeaderModel()
                    {
                        SectionList = items.Select(c => new SectionModel()
                        {
                            SectionText = c.GetEntityName(culture),
                            SectionTitle = c.GetEntityName(culture),
                            Href = urlService.GetUrl(c, new CategoryUrlArgs(globalModel.ChannelSystemId)),
                            SectionLinkList = categoryService.GetChildCategories(c.SystemId).Select(subCategory => new LinkModel()
                            {
                                Href = urlService.GetUrl(subCategory, new CategoryUrlArgs(globalModel.ChannelSystemId)),
                                Text = subCategory.GetEntityName(culture)
                            }).ToList()
                        }).ToList()
                    };
                });
        }
    }
}
