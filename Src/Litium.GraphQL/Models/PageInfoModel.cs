using Litium.Runtime.AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace Litium.GraphQL.Models
{
    public class PageInfoModel<TModel, TViewModel> where TModel : class
    {
        public int Total { get; set; }
        public bool HasNextPage { get; set; }
        public IEnumerable<TViewModel> List { get; set; }

        public PageInfoModel() { }

        public PageInfoModel(IEnumerable<TModel> list, int offset, int take)
        {
            Total = list.Count();
            List = list.Skip(offset).Take(take).MapEnumerableTo<TViewModel>();
            HasNextPage = offset + take < Total;
        }
    }
}
