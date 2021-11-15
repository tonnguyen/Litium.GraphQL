using System.Linq;
using Litium.Sales;
using Litium.Validations;
using Litium.StateTransitions;

namespace Litium.Accelerator.StateTransitions
{
    public class InitToConfirmedCondition : StateTransitionValidationRule<SalesOrder>
    {
        private readonly OrderOverviewService _orderOverviewService;

        public InitToConfirmedCondition(OrderOverviewService orderOverviewService)
        {
            _orderOverviewService = orderOverviewService;
        }

        public override string FromState => Sales.OrderState.Init;

        public override string ToState => Sales.OrderState.Confirmed;

        public override ValidationResult Validate(SalesOrder entity)
        {
            var result = new ValidationResult();
            //Administrator is not allowed to Confirm orders, they are automatically confirmed when payment is done.
            // At least one payment is guaranteed. In paymentInfo, there is at least one transaction that has TransactionType = Authorize
            // and TransactionResult = success.
            var isPaymentGuaranteed = _orderOverviewService.Get(entity.SystemId)
                .PaymentOverviews.Any(x => x.Transactions.Any(t => t.TransactionType == TransactionType.Authorize
                                                                    && t.TransactionResult == TransactionResult.Success));
            if (!isPaymentGuaranteed)
            {
                result.AddError("Payment", "Payment was not guaranteed.");
            }

            return result;
        }
    }
}
