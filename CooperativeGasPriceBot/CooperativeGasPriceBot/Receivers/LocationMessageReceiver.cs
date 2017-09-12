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

namespace CooperativeGasPriceBot.Receivers
{
    public class LocationMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;

        public LocationMessageReceiver(
            IMessagingHubSender sender
            )
        {
            _sender = sender;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var location = envelope.Content as Location;
            await _sender.SendMessageAsync($"Localização recebida!\nLat: {location.Latitude};\nLon: {location.Longitude}", envelope.From, cancellationToken);
        }
    }
}
