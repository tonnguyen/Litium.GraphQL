using Litium.Runtime.AutoMapper;
using System;
using System.Collections.Generic;
using AutoMapper;
using Litium.Websites;

namespace Litium.GraphQL.Models
{
    public class PageModel : IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Name { get; set; }
        public string TemplateId { get; set; }
        public IList<BlockAreaModel> Areas { get; set; }
        public string Slug { get; set; }
        public IList<PageModel> Children {get; set; }
        public string ValueAsJSON { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Page, PageModel>()
                .ForMember(x => x.Name, m => m.Ignore())
                .ForMember(x => x.Areas, m => m.Ignore())
                .ForMember(x => x.Children, m => m.Ignore())
                ;
        }
    }
}