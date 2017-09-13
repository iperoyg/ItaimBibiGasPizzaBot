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

namespace CooperativeGasPriceBot.Receivers
{
    public class ReceivePriceSetMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IGasStationService _gasStationService;
        private readonly IUserContextService _context;
        private readonly IStateManager _state;

        public ReceivePriceSetMessageReceiver(
            IMessagingHubSender sender,
            IGasStationService gasStationService,
            IUserContextService context,
            IStateManager state

            )
        {
            _sender = sender;
            _gasStationService = gasStationService;
            _context = context;
            _state = state;
        }


        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var endMenu = new Select
            {
                Scope = SelectScope.Immediate,
                Text = "Para finalizar, clique no botão baixo. Para voltar ao início, clique em menu.",
                Options = new SelectOption[]
                {
                    new SelectOption { Text = "Finalizar", Value = PlainText.Parse("/end") },
                    new SelectOption { Text = "Menu", Value = PlainText.Parse("/menu") }
                }
            };
            try
            {
                var content = (envelope.Content as PlainText).Text;
                if (content == "/stop")
                {
                    await _sender.SendMessageAsync("Atualização de preço encerrada!", userNode, cancellationToken);
                    await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
                    await _state.ResetStateAsync(userNode);
                    return;
                }
                var price = decimal.Parse((envelope.Content as PlainText).Text.Replace(',', '.'));
                var context = await _context.GetContextAsync(envelope.From.ToIdentity(), cancellationToken);
                var station = await _gasStationService.GetGasStationByIdAsync(context.CurrentGasStationId, cancellationToken);
                station.ActualPrice = price;
                await _gasStationService.UpdateGasStationAsync(station, cancellationToken);

                await _sender.SendMessageAsync("Preço atualizado com sucesso!", userNode, cancellationToken);
                await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
                await _state.ResetStateAsync(userNode);
            }
            catch (Exception)
            {
                await _sender.SendMessageAsync("Parece que essa mensagem não é um preço. Digite um preço válido ou '/stop' para parar.", userNode, cancellationToken);
            }

        }
    }
}
