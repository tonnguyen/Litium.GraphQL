using GraphQL.Types;
using Litium.GraphQL.Models;
using Litium.Runtime.DependencyInjection;

namespace Litium.GraphQL.Types
{
    [Service]
    public class SectionType : ObjectGraphType<SectionModel>
    {
        public SectionType()
        {
            Name = "Section";
            Field(p => p.SectionText);
            Field(p => p.SectionTitle);
            Field(p => p.Href);

            Field<ListGraphType<LinkGraphType>>(nameof(SectionModel.SectionLinkList),
                resolve: context =>
                {
                    return context.Source.SectionLinkList;
                });
        }
    }
}