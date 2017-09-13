using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using Lime.Messaging.Contents;
using CooperativeGasPriceBot.Services;
using Takenet.MessagingHub.Client.Extensions.Resource;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Extensions.Broadcast;
using Takenet.MessagingHub.Client.Extensions.EventTracker;

namespace CooperativeGasPriceBot.Receivers
{
    public class SetLoveGasStationMessageReceiver : BaseMessageReceiver
    {
        private readonly IGasStationService _gasStationService;
        private readonly IBroadcastExtension _broad;

        public SetLoveGasStationMessageReceiver(
            IUserContextService context,
            IResourceExtension resource,
            IMessagingHubSender sender,
            Settings settings,
            IGasStationService gasStationService,
            IBroadcastExtension broad,
            IEventTrackExtension track,
            IContactService contactService
            ) : base(sender, contactService, resource, context, track, settings)
        {
            _gasStationService = gasStationService;
            _broad = broad;
        }

        public override async Task ProcessAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var content = (envelope.Content as PlainText).Text;
            var tokens = content.Split('/');
            var action = tokens[1];
            var id = int.Parse(tokens.LastOrDefault());
            var station = await _gasStationService.GetGasStationByIdAsync(id, cancellationToken);
            var context = await _context.GetContextAsync(userNode, cancellationToken);

            string actionMessage = "";
            if (action == "love")
            {
                await _track.AddAsync("Habilidades", "Favoritar", cancellationToken: cancellationToken, identity: userNode);
                context.LovedGasStations.Add(id);
                context.LovedGasStations = context.LovedGasStations.Distinct().ToList();
                actionMessage = $"'{station.Name}' adicionado com sucesso à lista de postos favoritos.";
                await _broad.AddRecipientAsync(station.GetNameId(), userNode, cancellationToken);
            }
            else // action == unlove
            {
                await _track.AddAsync("Habilidades", "Desfavoritar", cancellationToken: cancellationToken, identity: userNode);
                context.LovedGasStations.RemoveAll(i => i == id);
                actionMessage = $"'{station.Name}' removido com sucesso de sua lista de postos favoritos.";
                await _broad.DeleteRecipientAsync(station.GetNameId(), userNode, cancellationToken);
            }

            await _context.SetContextAsync(context, userNode, cancellationToken);
            await _sender.SendMessageAsync(actionMessage, userNode, cancellationToken);
            var endMenu = await _resource.GetAsync<Document>(_settings.Resources.EndMenu, cancellationToken);
            await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
        }

    }
}
