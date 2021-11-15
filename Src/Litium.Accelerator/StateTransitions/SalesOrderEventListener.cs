using Litium.Events;
using Litium.Runtime;
using System.Threading.Tasks;
using System.Threading;
using Litium.Accelerator.Services;
using Litium.Sales.Events;
using Litium.Accelerator.Mailing;

namespace Litium.Accelerator.StateTransitions
{
    [Autostart]
    public class SalesOrderEventListener : IAsyncAutostart
    {
        private readonly EventBroker _eventBroker;
        private readonly MailService _mailService;

        public SalesOrderEventListener(
            EventBroker eventBroker,
            MailService mailService)
        {
            _eventBroker = eventBroker;
            _mailService = mailService;
        }

        ValueTask IAsyncAutostart.StartAsync(CancellationToken cancellationToken)
        {
            _eventBroker.Subscribe<SalesOrderConfirmed>(x =>
            {
                _mailService.SendEmail(new OrderConfirmationEmail(x.Item.ChannelSystemId.Value, x.SystemId, x.Item.CustomerInfo.Email), false);
            });

            return ValueTask.CompletedTask;
        }
    }
}
