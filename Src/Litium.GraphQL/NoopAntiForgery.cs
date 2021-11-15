using System.Threading.Tasks;
using Litium.Runtime.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Litium.Accelerator.GraphQL
{
    [Service(ServiceType = typeof(IAntiforgery))]
    internal class NoopAntiforgery : IAntiforgery
    {
        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            throw new System.NotImplementedException();
        }

        public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        {
            throw new System.NotImplementedException();
        }

        public void SetCookieTokenAndHeader(HttpContext httpContext)
        {
            throw new System.NotImplementedException();
        }

        public Task ValidateRequestAsync(HttpContext httpContext)
        {
            throw new System.NotImplementedException();
        }
    }
}