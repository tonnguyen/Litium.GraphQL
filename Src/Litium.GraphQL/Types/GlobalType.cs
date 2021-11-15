using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class GlobalType : ObjectGraphType<GlobalModel>
    {
        public GlobalType()
        {
            Name = "Global";
            Description = "Site global data";
            Field(p => p.ChannelSystemId, type: typeof(IdGraphType)).Description("The Channel system Id");
            Field(p => p.WebsiteSystemId, type: typeof(IdGraphType)).Description("The Website system Id");
            Field(p => p.CurrencySystemId, type: typeof(IdGraphType)).Description("The Currency system Id");
            Field(p => p.CountrySystemId, type: typeof(IdGraphType)).Description("The Country system Id");
            Field(p => p.AssortmentSystemId, type: typeof(IdGraphType)).Description("The Assortment system Id");
            Field(p => p.CurrentCulture).Description("The current culture");
            Field(p => p.CurrentUICulture).Description("The current UI culture");
            Field(p => p.LogoUrl);
        }
    }
}