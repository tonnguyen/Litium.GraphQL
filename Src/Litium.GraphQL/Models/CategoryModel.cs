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

namespace Litium.GraphQL.Models
{
    public class CategoryModel : IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Images { get; set; }
        public IList<ProductModel> Products { get; set; }
        public IList<CategoryModel> Categories { get; set; }

        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Category, CategoryModel>()
                .ForMember(x => x.Products, m => m.Ignore())
                .ForMember(x => x.Categories, m => m.Ignore())
                .ForMember(x => x.Images, m => m.MapFrom<ImagesResolver>())
                .ForMember(x => x.Name, m => m.MapFrom(x => x.GetEntityName(true) ?? x.Id ?? "general.NameIsMissing".AsAngularResourceString()));
        }

        private class ImagesResolver : IValueResolver<Category, CategoryModel, IList<string>>
        {
            private readonly FileService _fileService;

            public ImagesResolver(FileService fileService)
            {
                _fileService = fileService;
            }

            public IList<string> Resolve(Category source, CategoryModel destination, IList<string> destMember, ResolutionContext context)
            {
                return source.Fields.GetImageUrls(_fileService, (i) => {
                    return new ImageSize() { MinSize = new Size(200, 200), MaxSize = new Size(400, 400) };
                });
            }
        }
    }
}