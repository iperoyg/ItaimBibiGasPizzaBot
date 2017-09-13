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

namespace CooperativeGasPriceBot.Receivers
{
    public class SetLoveGasStationMessageReceiver : IMessageReceiver
    {
        private readonly IUserContextService _context;
        private readonly IResourceExtension _resource;
        private readonly IMessagingHubSender _sender;
        private readonly Settings _settings;
        private readonly IGasStationService _gasStationService;

        public SetLoveGasStationMessageReceiver(
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
            var content = (envelope.Content as PlainText).Text;
            var tokens = content.Split('/');
            var id = int.Parse(tokens.LastOrDefault());
            var station = await _gasStationService.GetGasStationByIdAsync(id, cancellationToken);
            var context = await _context.GetContextAsync(userNode, cancellationToken);
            context.LovedGasStations.Add(id);
            context.LovedGasStations = context.LovedGasStations.Distinct().ToList();
            await _context.SetContextAsync(context, userNode, cancellationToken);
            await _sender.SendMessageAsync($"'{station.Name}' adicionado com sucesso à lista de postos favoritos.", userNode, cancellationToken);
            var endMenu = await _resource.GetAsync<Document>(_settings.Resources.EndMenu, cancellationToken);
            await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
        }
    }
}
