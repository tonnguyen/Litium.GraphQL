using Litium.Accelerator.Builders;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Websites;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Litium.GraphQL.Builders
{
    public interface IPageDataBuilder<T1> : IPageDataBuilder 
        where T1 : IViewModel
    {
        Task<T1> BuildAsync(IServiceScope scope, PageModel pageModel);
    }

    [Service(ServiceType = typeof(IPageDataBuilder), Lifetime = DependencyLifetime.Scoped, NamedService = true)]
    public interface IPageDataBuilder
    {
    }
}
