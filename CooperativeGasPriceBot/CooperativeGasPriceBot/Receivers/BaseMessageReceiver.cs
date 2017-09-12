using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Extensions.Contacts;
using Takenet.MessagingHub.Client.Extensions.Directory;
using Lime.Messaging.Resources;
using Lime.Protocol.Network;
using CooperativeGasPriceBot.Extensions;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client;
using CooperativeGasPriceBot.Services;

namespace CooperativeGasPriceBot.Receivers
{
    public class BaseMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IContactService _contactService;

        public BaseMessageReceiver(
            IMessagingHubSender sender,
            IContactService contactService
            )
        {
            _sender = sender;
            _contactService = contactService;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var contact = await GetContact(userNode, cancellationToken);

            if (contact.IsContactFirstTime())
            {
                await _sender.SendMessageAsync("Oi! Essa é a primeira vez que vc interage cmg!", userNode, cancellationToken);
            }
            else
            {
                await _sender.SendMessageAsync("Oi! Essa não é a sua primeira vez cmg!", userNode, cancellationToken);
            }

        }

        private async Task<Contact> GetContact(Identity userNode, CancellationToken cancellationToken)
        {
            return await _contactService.GetContactAsync(userNode, cancellationToken);
        }
    }
}
