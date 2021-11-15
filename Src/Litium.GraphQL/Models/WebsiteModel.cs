using System;

namespace Litium.GraphQL.Models
{
    public class WebsiteModel
    {
        public Guid SystemId { get; set; }
        public string LogoUrl { get; set; }
        public FooterModel Footer { get; set;}
        public HeaderModel Header { get; set; }
    }
}
