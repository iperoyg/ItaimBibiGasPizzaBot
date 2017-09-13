using CooperativeGasPriceBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Extensions.EventTracker;
using Takenet.MessagingHub.Client.Extensions.Resource;
using Takenet.MessagingHub.Client.Sender;
using Lime.Protocol;
using System.Threading;
using Takenet.MessagingHub.Client;

namespace CooperativeGasPriceBot.Receivers
{
    public class PlainTextMessageReceiver : BaseMessageReceiver
    { 
   
        public PlainTextMessageReceiver(
            IMessagingHubSender sender,
            IContactService contactService,
            IResourceExtension resource,
            Settings settings,
            IUserContextService context,
            IEventTrackExtension track
            ) : base(sender, contactService, resource, context, track, settings)
        {
        }

        public override async Task ProcessAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var menuMessageResource = await _resource.GetAsync<Document>(_settings.Resources.Menu, cancellationToken);
            await _sender.SendMessageAsync(menuMessageResource, userNode, cancellationToken);
        }
    }
}
