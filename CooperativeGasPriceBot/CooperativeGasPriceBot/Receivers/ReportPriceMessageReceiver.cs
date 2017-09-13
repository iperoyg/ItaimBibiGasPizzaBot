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
using Takenet.MessagingHub.Client.Extensions.Resource;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace CooperativeGasPriceBot.Receivers
{
    public class ReportPriceMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _userContextService;
        private readonly Settings _settings;
        private readonly IResourceExtension _resource;

        public ReportPriceMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService,
            Settings settings,
            IResourceExtension resource
            )
        {
            _sender = sender;
            _userContextService = userContextService;
            _settings = settings;
            _resource = resource;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var input = await _resource.GetAsync<Document>(_settings.Resources.ReportPriceLocationRequest, cancellationToken);
            await _sender.SendMessageAsync(input, userNode, cancellationToken);

            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);
            context = context ?? new UserContext();
            context.CurrentJourney = Journey.Report;
            await _userContextService.SetContextAsync(context, userNode, cancellationToken);
        }
    }
}
