using Litium.Accelerator.Builders.Block;
using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "ProductsAndBanner")]
    public class ProductsAndBannerBlockDataViewModelBuilder : IBlockDataBuilder<ProductsAndBannerBlockViewModel>
    {
        /// <summary>
        /// Build the mixed block view model
        /// </summary>
        /// <param name="blockModel">The current mixed block</param>
        /// <returns>Return the mixed block view model</returns>
        public virtual async Task<ProductsAndBannerBlockViewModel> BuildAsync(IServiceScope scope, BlockModel blockModel)
        {
            var builder = scope.ServiceProvider.GetService<ProductsAndBannerBlockViewModelBuilder>();
            return await builder.BuildAsync(blockModel);
        }
    }
}
