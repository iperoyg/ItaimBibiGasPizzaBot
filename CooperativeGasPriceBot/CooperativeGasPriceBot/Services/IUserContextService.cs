using System.Threading;
using System.Threading.Tasks;
using CooperativeGasPriceBot.Models;
using Lime.Protocol;

namespace CooperativeGasPriceBot.Services
{
    public interface IUserContextService
    {
        Task<UserContext> GetContextAsync(Identity user, CancellationToken cancellationToken);
        Task SetContextAsync(UserContext context, Identity user, CancellationToken cancellationToken);
    }
}