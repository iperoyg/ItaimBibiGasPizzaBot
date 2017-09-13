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
using CooperativeGasPriceBot.Services;
using Takenet.MessagingHub.Client.Extensions.Resource;
using Takenet.MessagingHub.Client.Extensions.Broadcast;

namespace CooperativeGasPriceBot.Receivers
{
    public class ReceivePriceSetMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IGasStationService _gasStationService;
        private readonly IUserContextService _context;
        private readonly IStateManager _state;
        private readonly IResourceExtension _resource;
        private readonly Settings _settings;
        private readonly IBroadcastExtension _broad;

        public ReceivePriceSetMessageReceiver(
            IMessagingHubSender sender,
            IGasStationService gasStationService,
            IUserContextService context,
            IStateManager state,
            IResourceExtension resource,
            Settings settings,
            IBroadcastExtension broad
            )
        {
            _sender = sender;
            _gasStationService = gasStationService;
            _context = context;
            _state = state;
            _resource = resource;
            _settings = settings;
            _broad = broad;
        }


        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var endMenu = await _resource.GetAsync<Document>(_settings.Resources.EndMenu, cancellationToken);
            try
            {
                var content = (envelope.Content as PlainText).Text;
                if (content == "/stop")
                {
                    var priceStop = await _resource.GetAsync<Document>(_settings.Resources.StopSetPrice, cancellationToken);
                    await _sender.SendMessageAsync(priceStop, userNode, cancellationToken);
                    await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
                    await _state.ResetStateAsync(userNode);
                    return;
                }
                var price = decimal.Parse((envelope.Content as PlainText).Text.Replace(',', '.'));
                var context = await _context.GetContextAsync(envelope.From.ToIdentity(), cancellationToken);
                var station = await _gasStationService.GetGasStationByIdAsync(context.CurrentGasStationId, cancellationToken);
                station.ActualPrice = price;
                await _gasStationService.UpdateGasStationAsync(station, cancellationToken);

                var priceUpdated = await _resource.GetAsync<Document>(_settings.Resources.PriceUpdated, cancellationToken);
                await _sender.SendMessageAsync(priceUpdated, userNode, cancellationToken);
                await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
                await _state.ResetStateAsync(userNode);

                await _broad.SendMessageAsync(station.GetNameId(), PlainText.Parse($"Preço do posto: '{station.Name}' atualizado para R$: {station.ActualPrice}"), cancellationToken: cancellationToken);

            }
            catch (Exception)
            {
                var notAPrice = await _resource.GetAsync<Document>(_settings.Resources.NotAPrice, cancellationToken);
                await _sender.SendMessageAsync(notAPrice, userNode, cancellationToken);
            }

        }
    }
}
