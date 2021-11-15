using Litium.Globalization;
using Litium.Web.Models;
using Litium.Web.Models.Products;
using System.Collections.Generic;
using Litium.Accelerator.Builders;
using Newtonsoft.Json;
using System;

namespace Litium.Accelerator.ViewModels.Product
{
    public class ProductItemViewModel : IViewModel
    {
        public Guid SystemId { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public Currency Currency { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        public string Id { get; set; }
        [JsonIgnore]
        public IList<ImageModel> Images { get; set; }
        public string[] ImageUrls { get; set; }
        public bool IsInStock { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public ProductPriceModel Price { get; set; }
        public string FormattedPrice { get; set; }
        public ProductModel Product { get; set; }
        public string QuantityFieldId { get; set; }
        public bool ShowBuyButton { get; set; }
        public bool ShowQuantityField { get; set; }
        public string Size { get; set; }
        public string StockStatusDescription { get; set; }
        public string Url { get; set; }
        public bool UseVariantUrl { get; set; }
    }
}
