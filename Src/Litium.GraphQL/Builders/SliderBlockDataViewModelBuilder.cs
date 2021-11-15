using Litium.Accelerator.Builders.Block;
using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "Slider")]
    public class SliderBlockDataViewModelBuilder : IBlockDataBuilder<SliderBlockViewModel>
    {
        private readonly SliderBlockViewModelBuilder _sliderBlockViewModelBuilder;

        public SliderBlockDataViewModelBuilder(SliderBlockViewModelBuilder sliderBlockViewModelBuilder)
        {
            _sliderBlockViewModelBuilder = sliderBlockViewModelBuilder;
        }

        public async Task<SliderBlockViewModel> BuildAsync(IServiceScope scope, BlockModel model)
        {
            return _sliderBlockViewModelBuilder.Build(model);
        }
    }
}
