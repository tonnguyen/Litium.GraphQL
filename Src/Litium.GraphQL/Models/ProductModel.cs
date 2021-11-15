using AutoMapper;
using Litium.Accelerator.ViewModels.Product;
using Litium.Runtime.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Litium.GraphQL.Models
{
    public class ProductModel : IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Images { get; set; }
        public string Slug { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public bool IsInStock { get; set; }
        public string FormattedPrice { get; set; }
        public bool ShowBuyButton { get; set; }
        public bool ShowQuantityField { get; set; }
        public string Size { get; set; }
        public string StockStatusDescription { get; set; }
        public bool UseVariantUrl { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ProductItemViewModel, ProductModel>()
                .ForMember(x => x.Images, m => m.MapFrom(x => x.ImageUrls.ToList()))
                .ForMember(x => x.Slug, m => m.MapFrom(x => x.Url))
                ;
            cfg.CreateMap<ProductPageViewModel, ProductModel>()
                .ForMember(x => x.Color, m => m.MapFrom(x => x.ColorText))
                .ForMember(x => x.Name, m => m.MapFrom(x => x.ProductItem.Name))
                .ForMember(x => x.Images, m => m.MapFrom(x => x.ProductItem.ImageUrls))
                .ForMember(x => x.Brand, m => m.MapFrom(x => x.BrandPage.Title))
                .ForMember(x => x.Id, m => m.MapFrom(x => x.ProductItem.Id))
                .ForMember(x => x.Slug, m => m.MapFrom(x => x.ProductItem.Url))
                .ForMember(x => x.Description, m => m.MapFrom(x => x.ProductItem.Description))
                .ForMember(x => x.FormattedPrice, m => m.MapFrom(x => x.ProductItem.Price.Price.FormatPrice(true)))
                ;
        }
    }

}