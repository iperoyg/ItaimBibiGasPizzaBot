using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using Lime.Messaging.Contents;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client;

namespace CooperativeGasPriceBot.Receivers
{
    public class SearchPriceMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;

        public SearchPriceMessageReceiver(
            IMessagingHubSender sender
            )
        {
            _sender = sender;
        }

        public async  Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
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
                    Value = PlainText.Parse("Ótimo!\nPara informar os preços próximos à vc, primeiro preciso da sua localização.")
                }
            };
            await _sender.SendMessageAsync(input, envelope.From, cancellationToken);
        }
    }
}
