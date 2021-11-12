using System;
using System.Linq;
using Litium.Sales;
using Litium.StateTransitions;
using Litium.Validations;

namespace Litium.Accelerator.StateTransitions.Shipment
{
    public class ProcessingToReadyToShipCondition : StateTransitionValidationRule<Sales.Shipment>
    {
        private readonly OrderOverviewService _orderOverviewService;

        public ProcessingToReadyToShipCondition(OrderOverviewService orderOverviewService)
        {
            _orderOverviewService = orderOverviewService;
        }

        public override string FromState => Sales.ShipmentState.Processing;

        public override string ToState => Sales.ShipmentState.ReadyToShip;

        public override ValidationResult Validate(Sales.Shipment entity)
        {
            var result = new ValidationResult();
            var orderOverview = _orderOverviewService.Get(entity.OrderSystemId);
            if (orderOverview == null)
            {
                throw new Exception($"The order for shipment ({entity.SystemId}) cannot be found.");
            }

            var capturedShipmentRows = orderOverview.PaymentOverviews
                                                    .SelectMany(payment => payment.Transactions
                                                                                  .Where(transaction => transaction.TransactionType == TransactionType.Capture
                                                                                                        && transaction.TransactionResult == TransactionResult.Success)
                                                                                  .SelectMany(capture => capture.Rows.Select(x => x.ShipmentRowSystemId)))
                                                                                  .ToList();

            //Make sure that all captures are completed successfully before moving the shipment to ReadyToShip.
            if (entity.Rows.Any(x => !capturedShipmentRows.Contains(x.SystemId)))
            {
                result.AddError("Shipment", "All captures are not completed.");
            }

            return result;
        }
    }
}
