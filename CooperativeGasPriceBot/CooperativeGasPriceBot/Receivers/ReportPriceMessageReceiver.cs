using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace CooperativeGasPriceBot.Receivers
{
    public class ReportPriceMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;

        public ReportPriceMessageReceiver(
            IMessagingHubSender sender
            )
        {
            _sender = sender;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var input = new Input
            {
                Validation = new InputValidation
                {
                    Rule = InputValidationRule.Type,
                    Type = Location.MediaType,
                },
                Label = new DocumentContainer
                {
                    Value = PlainText.Parse("Certo... Para reportar um preço próximo de vc, informe sua localização.")
                }
            };
            await _sender.SendMessageAsync(input, envelope.From, cancellationToken);
        }
    }
}
