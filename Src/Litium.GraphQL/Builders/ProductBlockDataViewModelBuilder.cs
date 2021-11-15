using Litium.Accelerator.Builders.Block;
using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "Product")]
    public class ProductBlockDataViewModelBuilder : IBlockDataBuilder<ProductBlockViewModel>
    {
        public async Task<ProductBlockViewModel> BuildAsync(IServiceScope scope, BlockModel model)
        {
            var builder = scope.ServiceProvider.GetService<ProductBlockViewModelBuilder>();
            return await builder.BuildAsync(model);
        }
    }
}
