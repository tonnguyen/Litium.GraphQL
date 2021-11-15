using GraphQL.Types;
using Litium.Globalization;
using Litium.GraphQL.Models;
using Litium.Products;
using Litium.Runtime.DependencyInjection;
using Litium.Web;

namespace Litium.GraphQL.Types
{
    [Service]
    public class ProductType : ObjectGraphType<ProductModel>
    {
        public ProductType(
            VariantService variantService,
            CategoryService categoryService,
            CurrencyService currencyService,
            ChannelService channelService,
            CountryService countryService,
            Web.Models.Products.ProductPriceModelBuilder productPriceModelBuilder,
            BaseProductService baseProductService,
            UrlService urlService)
        {
            Name = "Product";
            Description = "A product";
            Field(p => p.SystemId, type: typeof(IdGraphType)).Description("The system Id");
            Field(p => p.Id).Description("The article number");
            Field(p => p.Name);
            Field(p => p.Images);
            Field(p => p.Brand, nullable: true);
            Field(p => p.Color);
            Field(p => p.Description);
            Field(p => p.IsInStock);
            Field(p => p.FormattedPrice);
            Field(p => p.ShowBuyButton);
            Field(p => p.ShowQuantityField);
            Field(p => p.Size);
            Field(p => p.StockStatusDescription);
            Field(p => p.Slug);
            Field(p => p.UseVariantUrl);
        }
    }

    [Service]
    public class PageInfoProductType : PageInfoTypeBase<ProductType, BaseProduct, ProductModel> { }
}