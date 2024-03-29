﻿using JetBrains.Annotations;
using Litium.Products;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(StockService))]
    [RequireServiceImplementation]
    public abstract class StockService
    {
        /// <summary>
        /// Gets the stock status description.
        /// </summary>
        /// <param name="webSite">The web site.</param>
        /// <param name="variant">The variant.</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns>System.String.</returns>
        public abstract string GetStockStatusDescription([NotNull] Variant variant, string sourceId = null);

        /// <summary>
        /// Determines whether the specified variant has stock.
        /// </summary>
        /// <param name="variant">The variant.</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns><c>true</c> if the specified variant has stock; otherwise, <c>false</c>.</returns>
        public abstract bool HasStock([NotNull] Variant variant, string sourceId = null);
    }
}
