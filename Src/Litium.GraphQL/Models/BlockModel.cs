using Litium.Runtime.AutoMapper;
using System;
using AutoMapper;
using Litium.Blocks;

namespace Litium.GraphQL.Models
{
    public class BlockModel : IAutoMapperConfiguration
    {
        public Guid PageSystemId { get; set; }
        public Guid SystemId { get; set; }
        public string BlockType { get; set; }
        public string ValueAsJSON { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Block, BlockModel>()
                .ForMember(x => x.ValueAsJSON, m => m.Ignore())
                .ForMember(x => x.PageSystemId, m => m.Ignore())
                ;
        }
    }
}