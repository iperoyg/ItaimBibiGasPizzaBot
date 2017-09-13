using CooperativeGasPriceBot.Extensions;
using Lime.Messaging.Resources;
using Lime.Protocol;
using Lime.Protocol.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Extensions.Contacts;
using Takenet.MessagingHub.Client.Extensions.Directory;

namespace CooperativeGasPriceBot.Services
{
    public class ContactService : IContactService
    {
        private const string FirstTimeKey = "firstTime";
        private readonly IContactExtension _contact;
        private readonly IDirectoryExtension _directory;

        public ContactService(
            IContactExtension contact,
            IDirectoryExtension directory
            )
        {
            _contact = contact;
            _directory = directory;
        }

        public async Task<Contact> GetContactAsync(Identity identity, CancellationToken cancellationToken)
        {
            Contact contact = null;

            try
            {
                contact = await _contact.GetAsync(identity, cancellationToken);
                contact.Extras = contact.Extras ?? new Dictionary<string, string>();
                if (!contact.Extras.ContainsKey(FirstTimeKey))
                {
                    contact.Extras.Add(FirstTimeKey, false.ToString());
                }
                contact.Extras[FirstTimeKey] = false.ToString();
            }
            catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND)
            {
                // Contact does not exists
                // Call directory - BLiP add contact to contact list automatically when you do that
                var account = await _directory.GetDirectoryAccountAsync(identity, cancellationToken);
            }

            if (contact == null)
            {
                contact = await _contact.GetAsync(identity, cancellationToken);
                contact.Extras = contact.Extras ?? new Dictionary<string, string>();
                if (!contact.Extras.ContainsKey(FirstTimeKey))
                {
                    contact.Extras.Add(FirstTimeKey, true.ToString());
                }
                contact.Extras[FirstTimeKey] = true.ToString();
            }

            return contact;
        }

        public bool IsContactFirstTime(Contact contact)
        {
            return contact.IsContactFirstTime();
        }

    }
}
