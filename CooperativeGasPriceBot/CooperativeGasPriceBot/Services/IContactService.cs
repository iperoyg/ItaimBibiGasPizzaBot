using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Resources;
using Lime.Protocol;

namespace CooperativeGasPriceBot.Services
{
    public interface IContactService
    {
        Task<Contact> GetContactAsync(Identity identity, CancellationToken cancellationToken);
        bool IsContactFirstTime(Contact contact);
    }
}