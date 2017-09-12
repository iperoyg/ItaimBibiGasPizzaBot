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
using CooperativeGasPriceBot.Models;
using CooperativeGasPriceBot.Services;

namespace CooperativeGasPriceBot.Receivers
{
    public class SearchPriceMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _userContextService;

        public SearchPriceMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService
            )
        {
            _sender = sender;
            _userContextService = userContextService;
        }

        public async  Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
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
                    Value = PlainText.Parse("Ótimo!\nPara informar os preços próximos à vc, primeiro preciso da sua localização.")
                }
            };
            await _sender.SendMessageAsync(input, userNode, cancellationToken);
            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);
            context = context ?? new UserContext();
            context.CurrentJourney = Journey.Search;
            await _userContextService.SetContextAsync(context, userNode, cancellationToken);
        }
    }
}
