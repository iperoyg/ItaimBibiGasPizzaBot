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

namespace CooperativeGasPriceBot.Receivers
{
    public class BaseMessageReceiver : IMessageReceiver
    {
        private readonly IContactExtension _contact;
        private readonly IDirectoryExtension _directory;
        private readonly IMessagingHubSender _sender;

        public BaseMessageReceiver(
            IContactExtension contact,
            IDirectoryExtension directory,
            IMessagingHubSender sender
            )
        {
            _contact = contact;
            _directory = directory;
            _sender = sender;
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
            Contact contact = null;

            try
            {
                contact = await _contact.GetAsync(userNode, cancellationToken);
                contact.Extras = contact.Extras ?? new Dictionary<string, string>();
                if (!contact.Extras.ContainsKey("firstTime"))
                {
                    contact.Extras.Add("firstTime", false.ToString());
                }
                contact.Extras["firstTime"] = false.ToString();
            }
            catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND)
            {
                // Contact does not exists
                // Call directory - BLiP add contact to contact list automatically when you do that
                var account = await _directory.GetDirectoryAccountAsync(userNode, cancellationToken);
            }

            if (contact == null)
            {
                contact = await _contact.GetAsync(userNode, cancellationToken);
                contact.Extras = contact.Extras ?? new Dictionary<string, string>();
                if (!contact.Extras.ContainsKey("firstTime"))
                {
                    contact.Extras.Add("firstTime", true.ToString());
                }
                contact.Extras["firstTime"] = true.ToString();
            }

            return contact;
        }
    }
}
