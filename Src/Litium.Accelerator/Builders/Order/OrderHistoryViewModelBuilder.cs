using System.Collections.Generic;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Utilities;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.Order;
using Litium.Data;
using Litium.Runtime.AutoMapper;
using Litium.Security;
using Litium.Web;
using Litium.Web.Models.Websites;
using Litium.Sales;
using Litium.Sales.Queryable;
using System.Linq;

namespace Litium.Accelerator.Builders.Order
{
    public class OrderHistoryViewModelBuilder : IViewModelBuilder<OrderHistoryViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly SecurityContextService _securityContextService;
        private readonly UrlService _urlService;
        private readonly OrderViewModelBuilder _orderViewModelBuilder;
        private readonly PersonStorage _personStorage;
        private readonly DataService _dataserivce;
        private readonly OrderOverviewService _orderOverviewService;
        private const int DefaultNumberOfOrderPerPage = 10;

        public OrderHistoryViewModelBuilder(
            RequestModelAccessor requestModelAccessor,
            SecurityContextService securityContextService,
            UrlService urlService,
            OrderViewModelBuilder orderViewModelBuilder,
            PersonStorage personStorage,
            DataService dataserivce,
            OrderOverviewService orderOverviewService)
        {
            _requestModelAccessor = requestModelAccessor;
            _securityContextService = securityContextService;
            _urlService = urlService;
            _orderViewModelBuilder = orderViewModelBuilder;
            _personStorage = personStorage;
            _dataserivce = dataserivce;
            _orderOverviewService = orderOverviewService;
        }

        public virtual OrderHistoryViewModel Build(int pageIndex, bool showOnlyMyOrders)
            => Build(_requestModelAccessor.RequestModel.CurrentPageModel, pageIndex, showOnlyMyOrders);

        public virtual OrderHistoryViewModel Build(PageModel pageModel, int pageIndex, bool showOnlyMyOrders)
        {
            var model = pageModel.MapTo<OrderHistoryViewModel>();
            model.IsBusinessCustomer = _personStorage.CurrentSelectedOrganization != null;

            if(model.IsBusinessCustomer)
            {
                model.HasApproverRole = _personStorage.HasApproverRole;
                model.ShowOnlyMyOrders = showOnlyMyOrders;
                model.MyOrdersLink = GetMyOrdersLink(pageModel, showOnlyMyOrders);
            }

            var itemsPerPage = model.NumberOfOrdersPerPage > 0 ? model.NumberOfOrdersPerPage : DefaultNumberOfOrderPerPage;
            model.Orders = GetOrders(model, pageIndex, itemsPerPage, out int totalOrders);
            model.Pagination = new PaginationViewModel(totalOrders, pageIndex, itemsPerPage);

            return model;
        }

        private List<OrderDetailsViewModel> GetOrders(OrderHistoryViewModel model, int pageIndex, int pageSize, out int totalOrderCount)
        {
            var orders = new List<OrderDetailsViewModel>();
            totalOrderCount = 0;
            var personId = _securityContextService.GetIdentityUserSystemId();
            if (!personId.HasValue)
            {
                return orders;
            }
            using (var query = _dataserivce.CreateQuery<SalesOrder>())
            {
                var queryResult = query.Filter(filter =>
                {
                    if (model.IsBusinessCustomer)
                    {
                        filter.OrganizationSystemId(_personStorage.CurrentSelectedOrganization.SystemId);
                        if (model.ShowOnlyMyOrders || !model.HasApproverRole)
                        {
                            filter.PersonSystemId(personId.Value);
                        }
                    }
                    else
                    {
                        filter.PersonSystemId(personId.Value);
                    }
                })
                .Sort(descriptor => descriptor.OrderNumber(Data.Queryable.SortDirection.Descending));

                totalOrderCount = queryResult.Count();
                var orderSystemIds = queryResult
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToSystemIdList();
                orders = _orderOverviewService.Get(orderSystemIds).Select(x => _orderViewModelBuilder.Build(x)).ToList();
            }
            return orders;
        }

        private string GetMyOrdersLink(PageModel pageModel, bool showOnlyMyOrders)
        {
            var myOrdersLink = _urlService.GetUrl(pageModel.Page);
            return !showOnlyMyOrders ? $"{myOrdersLink}?showMyOrders={true}" : myOrdersLink;
        }
    }
}

