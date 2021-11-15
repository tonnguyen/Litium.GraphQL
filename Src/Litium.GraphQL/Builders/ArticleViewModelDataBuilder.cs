using Litium.Accelerator.Builders.Article;
using Litium.Accelerator.ViewModels.Article;
using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Websites;
using Microsoft.Extensions.DependencyInjection;

namespace Litium.GraphQL.Builders
{
    [Service(Name = "Article")]
    public class ArticleViewModelDataBuilder : IPageDataBuilder<ArticleViewModel>
    {
        private readonly ArticleViewModelBuilder _builder;

        public ArticleViewModelDataBuilder(ArticleViewModelBuilder builder)
        {
            _builder = builder;
        }

        public async System.Threading.Tasks.Task<ArticleViewModel> BuildAsync(IServiceScope scope, PageModel pageModel)
        {
            return _builder.Build(pageModel);
        }
    }
}
