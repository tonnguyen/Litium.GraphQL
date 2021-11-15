using Litium.Accelerator.Builders.Block;
using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "Banner")]
    public class BannersBlockDataViewModelBuilder : IBlockDataBuilder<BannersBlockViewModel>
    {
        private readonly BannersBlockViewModelBuilder _builder;

        public BannersBlockDataViewModelBuilder(BannersBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        public async Task<BannersBlockViewModel> BuildAsync(IServiceScope scope, BlockModel model)
        {
            return _builder.Build(model);
        }
    }
}
