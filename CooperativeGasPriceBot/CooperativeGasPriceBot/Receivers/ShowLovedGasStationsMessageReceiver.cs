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

namespace CooperativeGasPriceBot.Receivers
{
    public class ShowLovedGasStationsMessageReceiver : IMessageReceiver
    {
        private readonly IUserContextService _context;
        private readonly IGasStationService _gasStationService;
        private readonly Settings _settings;
        private readonly IMessagingHubSender _sender;
        private readonly IResourceExtension _resource;

        public ShowLovedGasStationsMessageReceiver(
            IUserContextService context,
            IResourceExtension resource,
            IMessagingHubSender sender,
            Settings settings,
            IGasStationService gasStationService
            )
        {
            _context = context;
            _resource = resource;
            _sender = sender;
            _settings = settings;
            _gasStationService = gasStationService;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
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
