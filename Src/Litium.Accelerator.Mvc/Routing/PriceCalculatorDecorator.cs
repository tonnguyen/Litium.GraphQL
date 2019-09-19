using JetBrains.Annotations;
using Litium.Globalization;
using Litium.Products;
using Litium.Products.PriceCalculator;
using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Litium.Accelerator.Mvc.Routing
{
    [ServiceDecorator(typeof(IPriceCalculator))]
    internal class PriceCalculatorDecorator : IPriceCalculator
    {
        private readonly IPriceCalculator _parentResolver;
        private readonly ChannelService _channelService;

        public PriceCalculatorDecorator(IPriceCalculator parentResolver, ChannelService channelService)
        {
            _parentResolver = parentResolver;
            _channelService = channelService;
        }

        public IDictionary<Guid, PriceCalculatorResult> GetListPrices([NotNull] PriceCalculatorArgs calculatorArgs, [NotNull] params PriceCalculatorItemArgs[] itemArgs)
        {
            SetCountrySystemId(calculatorArgs);
            return _parentResolver.GetListPrices(calculatorArgs, itemArgs);
        }

        public ICollection<PriceList> GetPriceLists([NotNull]PriceCalculatorArgs calculatorArgs)
        {
            SetCountrySystemId(calculatorArgs);
            return _parentResolver.GetPriceLists(calculatorArgs);
        }

        private void SetCountrySystemId([NotNull] PriceCalculatorArgs calculatorArgs)
        {
            if (calculatorArgs.CountrySystemId == Guid.Empty)
            {
                calculatorArgs.CountrySystemId = _channelService.GetAll().First().CountryLinks.First().CountrySystemId;
            }
        }
    }
}