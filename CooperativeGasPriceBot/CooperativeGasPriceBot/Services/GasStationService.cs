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
            var stations = new List<GasStation>
            {
                new GasStation { ActualPrice = default(decimal), Address = "Rua Lorem, 1 - Bairro A", Name = "Posto Bonito", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { ActualPrice = default(decimal), Address = "Rua Ipsum, 2 - Bairro B", Name = "Posto Amarelo", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { ActualPrice = default(decimal), Address = "Rua Neque, 3 - Bairro C", Name = "Posto Vegano", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { ActualPrice = default(decimal), Address = "Rua Quisquam, 4 - Bairro D", Name = "Posto d'Cima", PhotoUri = "http://0mn.io/Content/ia4xn"},
                new GasStation { ActualPrice = default(decimal), Address = "Rua Dolorem, 5 - Bairro E", Name = "Posto Invertido", PhotoUri = "http://0mn.io/Content/ia4xn"}
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
            var targetStation = stations.FirstOrDefault(s => s.Name == station.Name);
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

    }
}
