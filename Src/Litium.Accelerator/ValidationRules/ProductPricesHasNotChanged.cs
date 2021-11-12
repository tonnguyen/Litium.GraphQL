using System;
using System.Linq;
using Litium.Globalization;
using Litium.Sales;
using Litium.Sales.Factory;
using Litium.Security;
using Litium.Validations;
using Litium.Web;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    /// Validates whether product prices has not changed in cart, since last time an order calculation was done.
    /// </summary>
    public class ProductPricesHasNotChanged : ValidationRuleBase<ValidateCartContextArgs>
    {
        private readonly ISalesOrderRowFactory _salesOrderRowFactory;
        private readonly SecurityContextService _securityContextService;
        private readonly CountryService _countryService;
        private readonly CurrencyService _currencyService;

        public ProductPricesHasNotChanged(
            ISalesOrderRowFactory salesOrderRowFactory,
            SecurityContextService securityContextService,
            CountryService countryService,
            CurrencyService currencyService)
        {
            _salesOrderRowFactory = salesOrderRowFactory;
            _securityContextService = securityContextService;
            _countryService = countryService;
            _currencyService = currencyService;
        }

        public override ValidationResult Validate(ValidateCartContextArgs entity, ValidationMode validationMode)
        {
            var result = new ValidationResult();
            var order = entity.Cart.Order;

            if (order.Rows.Count > 0)
            {
                var personId = order.CustomerInfo?.PersonSystemId ?? _securityContextService.GetIdentityUserSystemId() ?? Guid.Empty;
                var orderRows = from orderRow in order.Rows.Where(x => x.OrderRowType == OrderRowType.Product)
                                let createdRow = _salesOrderRowFactory.Create(new CreateSalesOrderRowArgs
                                {
                                    ArticleNumber = orderRow.ArticleNumber,
                                    Quantity = orderRow.Quantity,
                                    PersonSystemId = personId,
                                    ChannelSystemId = order.ChannelSystemId ?? Guid.Empty,
                                    CountrySystemId = _countryService.Get(order.CountryCode)?.SystemId ?? Guid.Empty,
                                    CurrencySystemId = _currencyService.Get(order.CurrencyCode)?.SystemId ?? Guid.Empty
                                })
                                where createdRow != null
                                where orderRow.UnitPriceExcludingVat != createdRow.UnitPriceExcludingVat
                                      || orderRow.VatRate != createdRow.VatRate
                                select orderRow;

                if (orderRows.Any())
                {
                    result.AddError("Cart", "sales.validation.productprices.haschanged".AsWebsiteText());
                }
            }

            return result;
        }
    }
}
