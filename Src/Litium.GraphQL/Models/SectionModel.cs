using System.Collections.Generic;

namespace Litium.GraphQL.Models
{
    public class SectionModel
    {
        public IList<LinkModel> SectionLinkList { get; set; } = new List<LinkModel>();
        public string SectionText { get; set; }
        public string SectionTitle { get; set; }
        public string Href { get; set; }
    }
}
