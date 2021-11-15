using Litium.Accelerator.Builders.Block;
using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "Video")]
    public class VideoBlockDataViewModelBuilder : IBlockDataBuilder<VideoBlockViewModel>
    {
        private readonly VideoBlockViewModelBuilder _builder;

        public VideoBlockDataViewModelBuilder(VideoBlockViewModelBuilder builder)
        {
            _builder = builder;
        }

        public async Task<VideoBlockViewModel> BuildAsync(IServiceScope scope, BlockModel model)
        {
            return _builder.Build(model);
        }
    }
}
