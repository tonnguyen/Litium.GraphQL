using Litium.Accelerator.Builders.Block;
using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "Brand")]
    public class BrandsBlockDataViewModelBuilder : IBlockDataBuilder<BrandsBlockViewModel>
    {
        private readonly BrandsBlockViewModelBuilder _builder;

        public BrandsBlockDataViewModelBuilder(BrandsBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        public async Task<BrandsBlockViewModel> BuildAsync(IServiceScope scope, BlockModel model)
        {
            return _builder.Build(model);
        }
    }
}
