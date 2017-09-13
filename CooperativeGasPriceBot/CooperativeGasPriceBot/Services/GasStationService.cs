using CooperativeGasPriceBot.Models;
using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Extensions.Bucket;

namespace CooperativeGasPriceBot.Services
{
    public class GasStationService : IGasStationService
    {
        public static string GasStationsKey = nameof(GasStationsKey);

        private readonly IBucketExtension _bucket;

        public GasStationService(
            IBucketExtension bucket
            )
        {
            _bucket = bucket;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var stations = await GetAllAsync(cancellationToken);
            if(stations != null && stations.Count > 0)
            {
                return;
            }
            stations = new List<GasStation>
            {
                new GasStation { Id = 1, ActualPrice = default(decimal), Address = "Rua Lorem, 1 - Bairro A", Name = "Posto Bonito", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { Id = 5, ActualPrice = default(decimal), Address = "Rua Ipsum, 2 - Bairro B", Name = "Posto Amarelo", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { Id = 2, ActualPrice = default(decimal), Address = "Rua Neque, 3 - Bairro C", Name = "Posto Vegano", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { Id = 3, ActualPrice = default(decimal), Address = "Rua Quisquam, 4 - Bairro D", Name = "Posto d'Cima", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { Id = 4, ActualPrice = default(decimal), Address = "Rua Dolorem, 5 - Bairro E", Name = "Posto Invertido", PhotoUri = "http://0mn.io/Content/ia4xn"}
            };
            await SetAllAsync(stations, cancellationToken);
        }

        public async Task<List<GasStation>> GetGasStationNearLocationAsync(Location location, CancellationToken cancellationToken, bool withPrice = false)
        {
            var stations = (await GetAllAsync(cancellationToken))
                .Where(s => !withPrice || s.ActualPrice != default(decimal))
                .ToList();
            return stations;
        }

        public async Task UpdateGasStationAsync(GasStation station, CancellationToken cancellationToken)
        {
            var stations = await GetAllAsync(cancellationToken);
            var targetStation = stations.FirstOrDefault(s => s.Id == station.Id);
            targetStation.ActualPrice = station.ActualPrice;
            await SetAllAsync(stations, cancellationToken);
        }

        private async Task SetAllAsync(List<GasStation> stations, CancellationToken cancellationToken)
        {
            var collection = new DocumentCollection
            {
                ItemType = GasStation.MEDIA_TYPE,
                Items = stations.ToArray(),
                Total = stations.Count
            };
            await _bucket.SetAsync(GasStationsKey, collection, default(TimeSpan), cancellationToken);
        }

        private async Task<List<GasStation>> GetAllAsync(CancellationToken cancellationToken)
        {
            var collection = await _bucket.GetAsync<DocumentCollection>(GasStationsKey, cancellationToken);
            var gasStations = collection.Items.Select(i => i as GasStation).ToList();
            return gasStations;
        }

        public async Task<GasStation> GetGasStationByIdAsync(int id, CancellationToken cancellationToken)
        {
            var stations = await GetAllAsync(cancellationToken);
            var targetStation = stations.FirstOrDefault(s => s.Id == id);
            return targetStation;
        }

        public async Task<List<GasStation>> GetGasStationsByIdListAsync(List<int> lovedGasStations, CancellationToken cancellationToken)
        {
            var stations = await GetAllAsync(cancellationToken);
            return stations.Where(s => lovedGasStations.Contains(s.Id)).ToList();
        }

        public DocumentCollection GetCarrousel(List<GasStation> gasStations, Journey journey, UserContext context)
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
                        Options = GetCarrouselOptions(g, journey, context)
                    })
                    .ToArray(),
                Total = gasStations.Count
            };
            return doc;
        }

        private DocumentSelectOption[] GetCarrouselOptions(GasStation g, Journey journey, UserContext c)
        {
            if (journey == Journey.Search)
            {
                return new DocumentSelectOption[]
                {
                    GetLoveOption(g, c, 1),
                    GetShareOption(g, c, 2)
                };
            }
            if (journey == Journey.Report)
            {
                return new DocumentSelectOption[]
                {
                    GetSetPriceOption(g, c, 1)
                };
            }
            if (journey == Journey.ListLoved)
            {
                return new DocumentSelectOption[]
                {
                    GetLoveOption(g, c, 1),
                    GetSetPriceOption(g, c, 2),
                    GetShareOption(g, c, 3)
                };
            }
            return null;
        }

        private DocumentSelectOption GetLoveOption(GasStation g, UserContext context, int order)
        {
            var isLoved = context.LovedGasStations.Contains(g.Id);
            var command = isLoved ? "unlove" : "love";
            var label = isLoved ? "Desfavoritar" : "Favoritar";
            return new DocumentSelectOption
            {
                Order = order,
                Label = new DocumentContainer { Value = PlainText.Parse(label) },
                Value = new DocumentContainer { Value = PlainText.Parse($"/{command}/{g.Id}") }
            };
        }

        private DocumentSelectOption GetShareOption(GasStation g, UserContext context, int order)
        {
            return new DocumentSelectOption
            {
                Order = order,
                Label = new DocumentContainer { Value = new WebLink { Uri = new Uri("share:") } },
                Value = new DocumentContainer { Value = PlainText.Parse($"/share/{g.Id}") }
            };
        }

        private DocumentSelectOption GetSetPriceOption(GasStation g, UserContext context, int order)
        {
            return new DocumentSelectOption
            {
                Order = order,
                Label = new DocumentContainer { Value = PlainText.Parse(g.ActualPrice == default(decimal) ? "Informar" : "Atualizar") },
                Value = new DocumentContainer { Value = PlainText.Parse($"/priceset/{g.Id}") }
            };
        }

    }
}
