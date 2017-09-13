using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using CooperativeGasPriceBot.Services;
using Takenet.MessagingHub.Client.Extensions.Resource;
using Takenet.MessagingHub.Client.Sender;
using CooperativeGasPriceBot.Models;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Extensions.EventTracker;

namespace CooperativeGasPriceBot.Receivers
{
    public class ShowLovedGasStationsMessageReceiver : BaseMessageReceiver
    {
        private readonly IGasStationService _gasStationService;

        public ShowLovedGasStationsMessageReceiver(
            IUserContextService context,
            IResourceExtension resource,
            IMessagingHubSender sender,
            Settings settings,
            IGasStationService gasStationService,
            IEventTrackExtension track,
            IContactService contactService
            ) : base(sender, contactService, resource, context, track, settings)
        {
            _gasStationService = gasStationService;
        }

        public override async Task ProcessAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            await _track.AddAsync("Habilidades", "Listar favoritos", cancellationToken: cancellationToken, identity: userNode);
            var context = await _context.GetContextAsync(userNode, cancellationToken);
            if (context.LovedGasStations.Count == 0)
            {
                var nonLoved = await _resource.GetAsync<Document>(_settings.Resources.NoneLovedStations, cancellationToken);
                await _sender.SendMessageAsync(nonLoved, userNode, cancellationToken);
            }
            else
            {
                var stations = await _gasStationService.GetGasStationsByIdListAsync(context.LovedGasStations, cancellationToken);
                var carrousel = _gasStationService.GetCarrousel(stations, Journey.ListLoved, context);
                await _sender.SendMessageAsync(carrousel, userNode, cancellationToken);
            }

            var endMenu = await _resource.GetAsync<Document>(_settings.Resources.EndMenu, cancellationToken);
            await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
        }
        
    }
}
