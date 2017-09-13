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
using Takenet.MessagingHub.Client.Extensions.EventTracker;
using Takenet.MessagingHub.Client.Extensions.Resource;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;

namespace CooperativeGasPriceBot.Receivers
{
    public class ReportPriceMessageReceiver : BaseMessageReceiver
    {

        public ReportPriceMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService context,
            Settings settings,
            IResourceExtension resource,
            IEventTrackExtension track,
            IContactService contactService
            ) : base(sender, contactService, resource, context, track, settings)
        {
        }

        public override async Task ProcessAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            await _track.AddAsync("Habilidades", "Informar preço", cancellationToken: cancellationToken, identity: userNode);
            var input = await _resource.GetAsync<Document>(_settings.Resources.ReportPriceLocationRequest, cancellationToken);
            await _sender.SendMessageAsync(input, userNode, cancellationToken);

            var context = await _context.GetContextAsync(userNode, cancellationToken);
            context = context ?? new UserContext();
            context.CurrentJourney = Journey.Report;
            await _context.SetContextAsync(context, userNode, cancellationToken);
        }
    }
}
