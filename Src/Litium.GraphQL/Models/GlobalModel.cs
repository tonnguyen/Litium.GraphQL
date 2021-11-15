using System;

namespace Litium.GraphQL.Models
{
    public class GlobalModel
    {
        public Guid ChannelSystemId { get; set; }
        public Guid WebsiteSystemId { get; set; }
        public Guid CurrencySystemId { get; set; }
        public Guid CountrySystemId { get; set; }
        public Guid AssortmentSystemId { get; set; }
        public string CurrentCulture { get; set; }
        public string CurrentUICulture { get; set; }
        public string LogoUrl { get; set; }
    }
}