using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CooperativeGasPriceBot.Models;
using Lime.Messaging.Contents;
using Lime.Protocol;

namespace CooperativeGasPriceBot.Services
{
    public interface IGasStationService
    {
        Task<List<GasStation>> GetGasStationNearLocationAsync(Location location, CancellationToken cancellationToken, bool withPrice = false);
        Task InitializeAsync(CancellationToken cancellationToken);
        Task UpdateGasStationAsync(GasStation station, CancellationToken cancellationToken);
        Task<GasStation> GetGasStationByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<GasStation>> GetGasStationsByIdListAsync(List<int> lovedGasStations, CancellationToken cancellationToken);
        DocumentCollection GetCarrousel(List<GasStation> gasStations, Journey journey, UserContext context);
    }
}