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
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client;

namespace CooperativeGasPriceBot.Receivers
{
    public class PriceSetMessageReceiver : IMessageReceiver
    {
        private readonly IGasStationService _gasStationService;
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _context;

        public PriceSetMessageReceiver(
            IGasStationService gasStationService,
            IMessagingHubSender sender,
            IUserContextService context
            )
        {
            _gasStationService = gasStationService;
            _sender = sender;
            _context = context;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = (envelope.Content as PlainText).Text;
            var tokens = content.Split('/');
            var id = int.Parse(tokens.LastOrDefault());
            var station = await _gasStationService.GetGasStationByIdAsync(id, cancellationToken);

            await _sender.SendMessageAsync($"Informe o preço para o posto: {station.Name}", envelope.From, cancellationToken);
        }
    }
}
