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
        public static string ReceiveStationPriceState = nameof(ReceiveStationPriceState);

        private readonly IGasStationService _gasStationService;
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _context;
        private readonly IStateManager _state;

        public PriceSetMessageReceiver(
            IGasStationService gasStationService,
            IMessagingHubSender sender,
            IUserContextService context,
            IStateManager state
            )
        {
            _gasStationService = gasStationService;
            _sender = sender;
            _context = context;
            _state = state;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = (envelope.Content as PlainText).Text;
            var tokens = content.Split('/');
            var id = int.Parse(tokens.LastOrDefault());
            var station = await _gasStationService.GetGasStationByIdAsync(id, cancellationToken);
            var context = await _context.GetContextAsync(envelope.From.ToIdentity(), cancellationToken);
            context.CurrentGasStationId = id;
            await _context.SetContextAsync(context, envelope.From.ToIdentity(), cancellationToken);
            //TODO: Considerar enviar para o contato o nome da estação atual e passar a msg abaixo para recurso com parâmetro
            await _sender.SendMessageAsync($"Informe o preço para o posto: {station.Name}", envelope.From, cancellationToken);
            await _state.SetStateAsync(envelope.From.ToIdentity(), ReceiveStationPriceState);
        }
    }
}
