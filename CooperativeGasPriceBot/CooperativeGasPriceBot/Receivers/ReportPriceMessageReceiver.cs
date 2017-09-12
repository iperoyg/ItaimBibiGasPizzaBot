using CooperativeGasPriceBot.Models;
using CooperativeGasPriceBot.Services;
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
        private readonly IUserContextService _userContextService;

        public ReportPriceMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService
            )
        {
            _sender = sender;
            _userContextService = userContextService;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
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
            await _sender.SendMessageAsync(input, userNode, cancellationToken);

            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);
            context = context ?? new UserContext();
            context.CurrentJourney = Journey.Report;
            await _userContextService.SetContextAsync(context, userNode, cancellationToken);
        }
    }
}
