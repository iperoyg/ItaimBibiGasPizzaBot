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
using Takenet.MessagingHub.Client.Extensions.EventTracker;

namespace CooperativeGasPriceBot.Receivers
{
    public class SearchPriceMessageReceiver : BaseMessageReceiver
    {

        public SearchPriceMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService context,
            IResourceExtension resource,
            Settings settings,
            IEventTrackExtension track,
            IContactService contactService
            ) : base(sender, contactService, resource, context, track, settings)
        {

        }

        public override async Task ProcessAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            await _track.AddAsync("Habilidades", "Melhor preço", cancellationToken: cancellationToken, identity: userNode);
            var input = await _resource.GetAsync<Document>(_settings.Resources.SearchPriceLocationRequest, cancellationToken);

            await _sender.SendMessageAsync(input, userNode, cancellationToken);
            var context = await _context.GetContextAsync(userNode, cancellationToken);
            context = context ?? new UserContext();
            context.CurrentJourney = Journey.Search;
            await _context.SetContextAsync(context, userNode, cancellationToken);
        }
    }
}
