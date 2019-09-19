using GraphQL.Types;
using Litium.Globalization;
using Litium.GraphQL.Models;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using System;
using System.Linq;

namespace Litium.GraphQL.Types
{
    [Service]
    public class ProductType : ObjectGraphType<ProductModel>
    {
        public ProductType(
            VariantService variantService,
            CurrencyService currencyService,
            ChannelService channelService,
            Web.Models.Products.ProductPriceModelBuilder productPriceModelBuilder)
        {
            Name = "Product";
            Description = "A product";
            Field(p => p.SystemId, type: typeof(IdGraphType)).Description("The system Id");
            Field(p => p.Id).Description("The article number");
            Field(p => p.Name).Description("The product name");
            Field(p => p.Images).Description("The images");
            Field(p => p.IsVariant).Description("The flag to indicate the object is a variant or a base product");
            Field<ListGraphType<ProductType>>(nameof(ProductModel.Variants), "Variants belong to the product",
                arguments: new QueryArguments(
                        new QueryArgument<IntGraphType> { Name = "skip" },
                        new QueryArgument<IntGraphType> { Name = "take" }
                    ),
                resolve: context => variantService.GetByBaseProduct(context.Source.SystemId)
                                    .Skip(context.GetArgument("skip", 0))
                                    .Take(context.GetArgument("take", 10))
                                    .MapEnumerableTo<ProductModel>());
            Field<PriceType>(nameof(ProductModel.Price),
                resolve: context =>
                {
                    if (!context.Source.IsVariant)
                    {
                        return null;
                    }

                    var variant = variantService.Get(context.Source.SystemId);
                    var currency = currencyService.GetBaseCurrency();
                    var channel = channelService.GetAll().First();
                    var price = productPriceModelBuilder.Build(variant, currency, channel);
                    return new PriceModel()
                    {
                        Currency = price.Currency.Symbol,
                        ListPrice = price.Price?.PriceWithVat ?? 0,
                        FormattedPrice = price.Price?.FormatPrice(true) ?? string.Empty
                    };
                }
            );
        }
    }

    [Service]
    public class PageInfoProductType : PageInfoTypeBase<ProductType, BaseProduct, ProductModel> { }
}