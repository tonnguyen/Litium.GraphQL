using AutoMapper;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.ViewModels.Media;
using Litium.Media;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Studio.Extenssions;
using Litium.Web.Administration.FieldFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Litium.GraphQL.Models
{
    public class ProductModel : IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<ProductModel> Variants { get; set; }
        public IList<string> Images { get; set; }
        public bool IsVariant { get; set; }
        public PriceModel Price { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<BaseProduct, ProductModel>()
                .ForMember(x => x.IsVariant, m => m.MapFrom(x => false))
                .ForMember(x => x.Images, m => m.MapFrom<BaseProductImagesResolver>())
                .ForMember(x => x.Name, m => m.MapFrom(x => x.GetEntityName(true) ?? x.Id ?? "general.NameIsMissing".AsAngularResourceString()));
            cfg.CreateMap<Variant, ProductModel>()
                .ForMember(x => x.IsVariant, m => m.MapFrom(x => true))
                .ForMember(x => x.Variants, m => m.Ignore())
                .ForMember(x => x.Images, m => m.MapFrom<VariantImagesResolver>())
                .ForMember(x => x.Name, m => m.MapFrom(x => x.GetEntityName(true) ?? x.Id ?? "general.NameIsMissing".AsAngularResourceString()));
        }
        private class BaseProductImagesResolver : IValueResolver<BaseProduct, ProductModel, IList<string>>
        {
            private readonly FileService _fileService;
            private readonly VariantService _variantService;

            public BaseProductImagesResolver(FileService fileService, VariantService variantService)
            {
                _fileService = fileService;
                _variantService = variantService;
            }

            public IList<string> Resolve(BaseProduct source, ProductModel destination, IList<string> destMember, ResolutionContext context)
            {
                var variant = _variantService.GetByBaseProduct(source.SystemId).FirstOrDefault();
                if (variant != null)
                {
                    var images = variant.Fields.GetImageUrls(_fileService, (i) => {
                        return new ImageSize() { MinSize = new Size(200, 200), MaxSize = new Size(400, 400) };
                    });
                    if (images != null && images.Count > 0)
                    {
                        return images;
                    }
                }
                return source.Fields.GetImageUrls(_fileService, (i) => {
                    return new ImageSize() { MinSize = new Size(200, 200), MaxSize = new Size(400, 400) };
                });
            }
        }

        private class VariantImagesResolver : IValueResolver<Variant, ProductModel, IList<string>>
        {
            private readonly FileService _fileService;

            public VariantImagesResolver(FileService fileService)
            {
                _fileService = fileService;
            }

            public IList<string> Resolve(Variant source, ProductModel destination, IList<string> destMember, ResolutionContext context)
            {
                return source.Fields.GetImageUrls(_fileService, (i) => {
                    return new ImageSize() { MinSize = new Size(200, 200), MaxSize = new Size(400, 400) };
                });
            }
        }
    }

}