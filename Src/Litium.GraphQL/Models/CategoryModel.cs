using AutoMapper;
using Litium.Accelerator.ViewModels.Product;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using System;
using System.Collections.Generic;

namespace Litium.GraphQL.Models
{
    public class CategoryModel : IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        // public Guid ParentSystemId { get; set; }
        // public CategoryModel ParentCategory { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Images { get; set; }
        public IList<ProductModel> Products { get; set; }
        public IList<CategoryModel> Categories { get; set; }
        public string Slug { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Category, CategoryModel>()
                // .ForMember(x => x.ParentSystemId, m => m.MapFrom(x => x.ParentCategorySystemId))
                .ForMember(x => x.Products, m => m.Ignore())
                .ForMember(x => x.Categories, m => m.Ignore())
                .ForMember(x => x.Name, m => m.Ignore());

            cfg.CreateMap<CategoryItemViewModel, CategoryModel>();
        }
    }
}