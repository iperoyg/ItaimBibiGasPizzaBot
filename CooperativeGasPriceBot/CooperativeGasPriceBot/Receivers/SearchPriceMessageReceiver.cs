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
using Takenet.MessagingHub.Client.Extensions.Resource;

namespace CooperativeGasPriceBot.Receivers
{
    public class SearchPriceMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _userContextService;
        private readonly IResourceExtension _resource;
        private readonly Settings _settings;

        public SearchPriceMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService,
            IResourceExtension resource,
            Settings settings
            )
        {
            _sender = sender;
            _userContextService = userContextService;
            _resource = resource;
            _settings = settings;
        }

        public async  Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var input = await _resource.GetAsync<Document>(_settings.Resources.SearchPriceLocationRequest, cancellationToken);
            
            await _sender.SendMessageAsync(input, userNode, cancellationToken);
            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);
            context = context ?? new UserContext();
            context.CurrentJourney = Journey.Search;
            await _userContextService.SetContextAsync(context, userNode, cancellationToken);
        }
    }
}
