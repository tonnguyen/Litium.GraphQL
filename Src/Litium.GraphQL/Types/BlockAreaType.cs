using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class BlockAreaType : ObjectGraphType<BlockAreaModel>
    {
        public BlockAreaType()
        {
            Name = "BlockArea";
            Description = "Block area data";
            Field(p => p.Id, type: typeof(IdGraphType)).Description("The area Id");
            Field(p => p.Blocks, type: typeof(ListGraphType<BlockType>)).Description("The blocks");
        }
    }
}