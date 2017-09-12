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
using CooperativeGasPriceBot.Models;

namespace CooperativeGasPriceBot.Receivers
{
    public class LocationMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _userContextService;
        private readonly IGasStationService _gasStationService;

        public LocationMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService,
            IGasStationService gasStationService
            )
        {
            _sender = sender;
            _userContextService = userContextService;
            _gasStationService = gasStationService;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var location = envelope.Content as Location;
            var userNode = envelope.From.ToIdentity();

            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);

            //await _sender.SendMessageAsync($"Localização recebida!\nLat: {location.Latitude};\nLon: {location.Longitude}", userNode, cancellationToken);


            switch (context.CurrentJourney)
            {
                case Models.Journey.Search:
                    var gasStations = await _gasStationService.GetGasStationNearLocationAsync(location, cancellationToken, withPrice: true);
                    if (gasStations == null || !gasStations.Any())
                    {
                        await _sender.SendMessageAsync($"Não existem postos com preços informados próximos!", userNode, cancellationToken);
                    }
                    DocumentCollection carrousel = GetCarrousel(gasStations, context.CurrentJourney);
                    await _sender.SendMessageAsync($"Abaixo, no futuro, será possível ver os postos de gasolina mais próximos, com o preço informado.", userNode, cancellationToken);
                    await _sender.SendMessageAsync(carrousel, userNode, cancellationToken);

                    break;
                case Models.Journey.Report:
                    var gasStations2 = await _gasStationService.GetGasStationNearLocationAsync(location, cancellationToken, withPrice: false);
                    DocumentCollection carrousel2 = GetCarrousel(gasStations2, context.CurrentJourney);
                    await _sender.SendMessageAsync($"Abaixo, no futuro, será possível entrar com o nome do posto e o preço!", userNode, cancellationToken);
                    await _sender.SendMessageAsync(carrousel2, userNode, cancellationToken);
                    break;
                default:
                    await _sender.SendMessageAsync($"Something is wrong here!!!", userNode, cancellationToken);
                    break;
            }
        }

        private DocumentCollection GetCarrousel(List<GasStation> gasStations, Journey journey)
        {
            var doc = new DocumentCollection
            {
                ItemType = DocumentSelect.MediaType,
                Items = gasStations
                    .Select(g => new DocumentSelect
                    {
                        Header = new DocumentContainer
                        {
                            Value = new MediaLink
                            {
                                Title = $"{g.Name} - Preço: R$: {g.ActualPrice}",
                                Text = $"{g.Address}",
                                Uri = new Uri(g.PhotoUri),
                                Type = "image/jpeg"
                            }
                        },
                        Options = GetCarrouselOptions(g, journey)
                    })
                    .ToArray(),
                Total = gasStations.Count
            };
            return doc;
        }

        private DocumentSelectOption[] GetCarrouselOptions(GasStation g, Journey journey)
        {
            if (journey == Journey.Search)
            {
                return new DocumentSelectOption[]
                {
                    new DocumentSelectOption
                    {
                        Order = 1,
                        Label = new DocumentContainer { Value = PlainText.Parse("Favoritar") },
                        Value = new DocumentContainer { Value = PlainText.Parse($"/love/{g.Id}") }
                    },
                    new DocumentSelectOption
                    {
                        Order = 1,
                        Label = new DocumentContainer { Value = new WebLink {Uri = new Uri("share:"), Text = "Compartilhar"} },
                        Value = new DocumentContainer { Value = PlainText.Parse($"/share/{g.Id}") }
                    },
                };
            }
            if (journey == Journey.Report)
            {
                return new DocumentSelectOption[]
                {
                    new DocumentSelectOption
                    {
                        Order = 1,
                        Label = new DocumentContainer { Value = PlainText.Parse(g.ActualPrice == default(decimal) ? "Informar" : "Atualizar") },
                        Value = new DocumentContainer { Value = PlainText.Parse($"/priceset/{g.Id}") }
                    }
                };
            }
            return null;
        }
    }
}
