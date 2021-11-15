using GraphQL;
using GraphQL.Types;
using Litium.Accelerator.Builders;
using Litium.Application.Runtime;
using Litium.Blocks;
using Litium.FieldFramework;
using Litium.GraphQL.Builders;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Litium.GraphQL.Types
{
    [Service]
    public class BlockType : ObjectGraphType<BlockModel>
    {
        public BlockType(BlockService blockService, FieldTemplateService fieldTemplateService, 
            NamedServiceFactory<IBlockDataBuilder> blockDataBuilderFactory,
            RouteInfoService routeInfoService)
        {
            Name = "Block";
            Description = "Block data";
            Field(p => p.SystemId, type: typeof(IdGraphType)).Description("The block Id");

            Field<StringGraphType>(nameof(BlockModel.BlockType), "The block type",
                resolve: context =>
                {
                    var block = blockService.Get(context.Source.SystemId);
                    var template = fieldTemplateService.Get<BlockFieldTemplate>(block.FieldTemplateSystemId);
                    return template.Id;
                });

            FieldAsync<StringGraphType>(nameof(BlockModel.ValueAsJSON), "Value as JSON string",
                arguments: new QueryArguments(
                        new QueryArgument<GlobalInputType> { Name = "global", Description = "The global object" }
                    ),
                resolve: async context =>
                {
                    var block = blockService.Get(context.Source.SystemId);
                    var template = fieldTemplateService.Get<BlockFieldTemplate>(block.FieldTemplateSystemId);
                    // Special treatment for scoped services https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#scoped-services-with-a-singleton-schema-lifetime
                    // and make sure it is thread safe https://graphql-dotnet.github.io/docs/getting-started/dependency-injection/#thread-safety-with-scoped-services
                    using var scope = context.RequestServices.CreateScope();
                    var builder = scope.ServiceProvider.GetNamedService<IBlockDataBuilder>(template.Id);
                    if (builder == null)
                    {
                        return null;
                    }
                    var globalModel = context.GetArgument<GlobalModel>("global");
                    routeInfoService.Setup(globalModel, context.Source.PageSystemId);
                    var buildMethod = builder.GetType().GetMethod(nameof(IBlockDataBuilder<IViewModel>.BuildAsync));
                    var value = await (dynamic)buildMethod.Invoke(builder, 
                        new object[] { scope, new Web.Models.Blocks.BlockModel(block, block.Fields) });
                    var jsonSerializerSettings = ApplicationConverter.JsonSerializerSettings();
                    // jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    return JsonConvert.SerializeObject(value, jsonSerializerSettings);
                });
        }
    }
}