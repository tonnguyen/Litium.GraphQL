using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class PriceType : ObjectGraphType<PriceModel>
    {
        public PriceType()
        {
            Name = "Price";
            Field(p => p.Currency).Description("The currency code");
            Field(p => p.ListPrice).Description("The list price");
            Field(p => p.FormattedPrice).Description("The formatted price as string");
        }
    }
}
