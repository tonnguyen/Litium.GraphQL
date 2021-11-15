using Litium.Accelerator.Builders;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Blocks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    public interface IBlockDataBuilder<T1> : IBlockDataBuilder 
        where T1 : IViewModel
    {
        Task<T1> BuildAsync(IServiceScope scope, BlockModel model);
    }

    [Service(ServiceType = typeof(IBlockDataBuilder), Lifetime = DependencyLifetime.Scoped, NamedService = true)]
    public interface IBlockDataBuilder
    {
    }
}
