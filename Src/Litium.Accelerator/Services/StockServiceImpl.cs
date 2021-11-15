using Litium.Globalization;
using Litium.Products;
using Litium.Products.StockStatusCalculator;
using Litium.Runtime.DependencyInjection;
using Litium.Sales;
using Litium.Security;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(StockService))]
    internal class StockServiceImpl : StockService
    {
        private readonly IStockStatusCalculator _stockStatusCalculator;
        private readonly CartContextAccessor _cartContextAccessor;
        private readonly CountryService _countryService;
        private readonly SecurityContextService _securityContextService;

        public StockServiceImpl(
            IStockStatusCalculator stockStatusCalculator,
            CartContextAccessor cartContextAccessor,
            CountryService countryService,
            SecurityContextService securityContextService)
        {
            _stockStatusCalculator = stockStatusCalculator;
            _cartContextAccessor = cartContextAccessor;
            _countryService = countryService;
            _securityContextService = securityContextService;
        }

        public override string GetStockStatusDescription(Variant variant, string sourceId = null)
        {
            return GetStockStatus(variant, sourceId)?.Description ?? string.Empty;
        }

        public override bool HasStock(Variant variant, string sourceId = null)
        {
            var stock = GetStockStatus(variant, sourceId);
            return (stock != null && stock.InStockQuantity.HasValue && stock.InStockQuantity > 0m);
        }

        private StockStatusCalculatorResult GetStockStatus(Variant variant, string sourceId)
        {
            var cartContext = _cartContextAccessor.CartContext;

            var calculatorArgs = new StockStatusCalculatorArgs
            {
                UserSystemId = _securityContextService.GetIdentityUserSystemId().GetValueOrDefault(),
                SourceId = sourceId,
                CountrySystemId = _countryService.Get(cartContext?.CountryCode)?.SystemId
            };
            var calculatorItemArgs = new StockStatusCalculatorItemArgs
            {
                VariantSystemId = variant.SystemId,
                Quantity = decimal.One
            };

            return _stockStatusCalculator.GetStockStatuses(calculatorArgs, calculatorItemArgs).TryGetValue(variant.SystemId, out StockStatusCalculatorResult calculatorResult)
                ? calculatorResult
                : null;
        }
    }
}
