using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class GlobalInputType : InputObjectGraphType<GlobalModel>
    {
        public GlobalInputType()
        {
            Name = "GlobalInput";
            Field(p => p.ChannelSystemId);
            Field(p => p.WebsiteSystemId);
            Field(p => p.CurrencySystemId);
            Field(p => p.CountrySystemId);
            Field(p => p.AssortmentSystemId);
            Field(p => p.CurrentCulture);
            Field(p => p.CurrentUICulture);
        }
    }
}
