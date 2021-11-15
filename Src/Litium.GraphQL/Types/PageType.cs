using GraphQL;
using GraphQL.Types;
using Litium.Accelerator.Builders;
using Litium.Application.Runtime;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.GraphQL.Builders;
using Litium.GraphQL.Models;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Administration.FieldFramework;
using Litium.Websites;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace Litium.GraphQL.Types
{
    [Service]
    public class PageType : ObjectGraphType<PageModel>
    {
        public PageType(PageService pageService, BlockService blockService, UrlService urlService,
            FieldTemplateService fieldTemplateService, RouteInfoService routeInfoService)
        {
            Name = "Page";
            Description = "Page data";
            Field(p => p.SystemId, type: typeof(IdGraphType)).Description("The System Id");
            Field(p => p.Slug, nullable: true).Description("The slug")
                .Argument<GlobalInputType>("global")
                .Resolve(context =>
                {
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    var page = pageService.Get(context.Source.SystemId);
                    return urlService.GetUrl(page, new PageUrlArgs(globalModel.ChannelSystemId));
                });

            Field<ListGraphType<BlockAreaType>>(nameof(PageModel.Areas), "Block areas",
                resolve: context =>
                {
                    var page = pageService.Get(context.Source.SystemId);
                    return page.Blocks.Items.Select(container =>
                        new BlockAreaModel()
                        {
                            Id = container.Id,
                            Blocks = blockService.Get(container.Items.Select(block => ((BlockItemLink)block).BlockSystemId))
                                                .MapEnumerableTo<BlockModel>()
                                                .Select(b => { b.PageSystemId = context.Source.SystemId; return b; }).ToList()
                        }).ToList();
                });

            Field<StringGraphType>(nameof(PageModel.TemplateId), "Template id",
                resolve: context =>
                {
                    var page = pageService.Get(context.Source.SystemId);
                    var template = fieldTemplateService.Get<PageFieldTemplate>(page.FieldTemplateSystemId);
                    return template.Id;
                });

            Field<StringGraphType>(nameof(PageModel.Name), "The page name",
                arguments: new QueryArguments(
                     new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" }
                ),
                resolve: context =>
                {
                    var page = pageService.Get(context.Source.SystemId);
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    return page.GetEntityName(true, CultureInfo.GetCultureInfo(globalModel.CurrentCulture)) ?? page.Id ?? "general.NameIsMissing".AsAngularResourceString();
                });

            Field<PageInfoPageType>(nameof(PageModel.Children), description: "Sub pages",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "offset" },
                    new QueryArgument<IntGraphType> { Name = "take" }
                ),
                resolve: context =>
                            new PageInfoModel<Page, PageModel>(pageService.GetChildPages(context.Source.SystemId), 
                                context.GetArgument("offset", 0), context.GetArgument("take", 10)));

            FieldAsync<StringGraphType>(nameof(PageModel.ValueAsJSON), "Value as JSON string",
                arguments: new QueryArguments(
                        new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" }
                    ),
                resolve: async context =>
                {
                    var page = pageService.Get(context.Source.SystemId);
                    var template = fieldTemplateService.Get<PageFieldTemplate>(page.FieldTemplateSystemId);
                    // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
                    // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
                    using var scope = context.RequestServices.CreateScope();
                    var builder = scope.ServiceProvider.GetNamedService<IPageDataBuilder>(template.Id);
                    if (builder == null)
                    {
                        return null;
                    }
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    routeInfoService.Setup(globalModel, context.Source.SystemId);
                    var buildMethod = builder.GetType().GetMethod(nameof(IPageDataBuilder<IViewModel>.BuildAsync));
                    var value = await (dynamic)buildMethod.Invoke(builder, 
                        new object[] { scope, new Web.Models.Websites.PageModel(page, page.Fields) });
                    // ((IDictionary<string, object>)value).Remove("blocks");
                    var jsonSerializerSettings = ApplicationConverter.JsonSerializerSettings();
                    return JsonConvert.SerializeObject(value, jsonSerializerSettings);
                });
        }
    }

    [Service]
    public class PageInfoPageType : PageInfoTypeBase<PageType, Page, PageModel> { }
}