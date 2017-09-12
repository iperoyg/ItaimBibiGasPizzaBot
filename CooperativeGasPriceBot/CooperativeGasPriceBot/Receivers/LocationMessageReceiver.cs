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
    public class LocationMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _userContextService;

        public LocationMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService
            )
        {
            _sender = sender;
            _userContextService = userContextService;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var location = envelope.Content as Location;
            var userNode = envelope.From.ToIdentity();

            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);

            await _sender.SendMessageAsync($"Localização recebida!\nLat: {location.Latitude};\nLon: {location.Longitude}", userNode, cancellationToken);

            switch (context.CurrentJourney)
            {
                case Models.Journey.Search:
                    await _sender.SendMessageAsync($"Abaixo, no futuro, será possível ver os postos de gasolina mais próximos, com o preço informado.", userNode, cancellationToken);
                    break;
                case Models.Journey.Report:
                    await _sender.SendMessageAsync($"Abaixo, no futuro, será possível entrar com o nome do posto e o preço!", userNode, cancellationToken);
                    break;
                default:
                    await _sender.SendMessageAsync($"Something is wrong here!!!", userNode, cancellationToken);
                    break;
            }
        }
    }
}
