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
using Takenet.MessagingHub.Client.Extensions.Resource;

namespace CooperativeGasPriceBot.Receivers
{
    public class LocationMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IUserContextService _userContextService;
        private readonly IGasStationService _gasStationService;
        private readonly IResourceExtension _resource;
        private readonly Settings _settings;

        public LocationMessageReceiver(
            IMessagingHubSender sender,
            IUserContextService userContextService,
            IGasStationService gasStationService,
            IResourceExtension resource,
            Settings settings
            )
        {
            _sender = sender;
            _userContextService = userContextService;
            _gasStationService = gasStationService;
            _resource = resource;
            _settings = settings;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var location = envelope.Content as Location;
            var userNode = envelope.From.ToIdentity();

            var context = await _userContextService.GetContextAsync(userNode, cancellationToken);
            
            var endMenu = await _resource.GetAsync<Document>(_settings.Resources.EndMenu, cancellationToken);

            switch (context.CurrentJourney)
            {
                case Models.Journey.Search:
                    var gasStations = await _gasStationService.GetGasStationNearLocationAsync(location, cancellationToken, withPrice: true);
                    if (gasStations == null || !gasStations.Any())
                    {
                        var notFound = await _resource.GetAsync<Document>(_settings.Resources.NotFoundStations, cancellationToken); ;
                        await _sender.SendMessageAsync(notFound, userNode, cancellationToken);
                        var menu = await _resource.GetAsync<Document>(_settings.Resources.Menu, cancellationToken);
                        await _sender.SendMessageAsync(menu, userNode, cancellationToken);
                        return;
                    }
                    DocumentCollection carrousel = GetCarrousel(gasStations, context.CurrentJourney);
                    await _sender.SendMessageAsync(carrousel, userNode, cancellationToken);
                    await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);

                    break;
                case Models.Journey.Report:
                    var gasStations2 = await _gasStationService.GetGasStationNearLocationAsync(location, cancellationToken, withPrice: false);
                    DocumentCollection carrousel2 = GetCarrousel(gasStations2, context.CurrentJourney);
                    await _sender.SendMessageAsync(carrousel2, userNode, cancellationToken);
                    await _sender.SendMessageAsync(endMenu, userNode, cancellationToken);
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
                                Title = $"{g.Name} - Preço: R$ {g.ActualPrice}",
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
                        Label = new DocumentContainer { Value = new WebLink { Uri = new Uri("share:") } },
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
