using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Blocks;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Web.Models;
using Litium.Accelerator.Constants;
using Litium.Web.Models.Websites;
using Litium.Accelerator.Extensions;
using Litium.FieldFramework.FieldTypes;
using Newtonsoft.Json;

namespace Litium.Accelerator.ViewModels.Block
{
    public class BrandsBlockViewModel : IViewModel, IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Title { get; set; }
        public string LinkText { get; set; }
        [JsonIgnore]
        public List<LinkModel> Pages { get; set; } = new List<LinkModel>();
        public List<HyperLinkModel> PageUrls { get; set; }
        [JsonIgnore]
        public LinkModel Url { get; set; }
        public HyperLinkModel UrlLink { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<BlockModel, BrandsBlockViewModel>()
               .ForMember(x => x.Title, m => m.MapFromField(BlockFieldNameConstants.BlockTitle))
               .ForMember(x => x.LinkText, m => m.MapFromField(BlockFieldNameConstants.LinkText))
               .ForMember(x => x.Pages, m => m.MapFrom(ResolvePages))
               .ForMember(x => x.Url, m => m.MapFrom(brand => brand.GetValue<PointerPageItem>(BlockFieldNameConstants.Link).MapTo<LinkModel>()))
               .AfterMap((block, blockModel) => {
                   blockModel.PageUrls = blockModel.Pages.Select(p => 
                    new HyperLinkModel()
                    {
                        Href = p.Href,
                        Text = p.Text,
                        ImageUrl = p.Image?.GetUrlToImage(System.Drawing.Size.Empty, new System.Drawing.Size(200, -1)).Url
                    }).ToList();
                    blockModel.UrlLink = new HyperLinkModel()
                    {
                        Href = blockModel.Url?.Href,
                        Text = blockModel.Url?.Text,
                        ImageUrl = blockModel.Url?.Image?.GetUrlToImage(System.Drawing.Size.Empty, new System.Drawing.Size(200, -1)).Url
                    };
               });
        }

        protected object ResolvePages(BlockModel brandBlock, BrandsBlockViewModel model)
        {
            var rs = new List<LinkModel>();
            var brandPages = brandBlock.GetValue<IList<PointerItem>>(BlockFieldNameConstants.BrandsLinkList)?.OfType<PointerPageItem>().ToList() ?? new List<PointerPageItem>();

            foreach (var brandPage in brandPages)
            {
                var page = brandPage.MapTo<LinkModel>();

                if (page != null)
                {
                    var fileSystemId = brandPage.EntitySystemId.MapTo<PageModel>()?.GetValue<Guid>(PageFieldNameConstants.Image);
                    if (fileSystemId != null && fileSystemId != Guid.Empty)
                    {
                        page.Image = fileSystemId.MapTo<ImageModel>();
                    }

                    rs.Add(page);
                }
            }

            return rs;
        }
    }
}
