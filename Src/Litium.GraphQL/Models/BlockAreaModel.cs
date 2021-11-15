using System.Collections.Generic;

namespace Litium.GraphQL.Models
{
    public class BlockAreaModel
    {
        public string Id { get; set; }
        public IList<BlockModel> Blocks { get; set; }
    }
}