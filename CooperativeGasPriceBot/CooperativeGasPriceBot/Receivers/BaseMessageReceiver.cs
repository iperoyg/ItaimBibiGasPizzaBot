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
using Takenet.MessagingHub.Client.Extensions.Resource;
using Lime.Messaging.Contents;
using Takenet.MessagingHub.Client.Extensions.EventTracker;

namespace CooperativeGasPriceBot.Receivers
{
    public abstract class BaseMessageReceiver : IMessageReceiver
    {
        protected readonly IMessagingHubSender _sender;
        protected readonly IContactService _contactService;
        protected readonly IResourceExtension _resource;
        protected readonly IUserContextService _context;
        protected readonly IEventTrackExtension _track;
        protected readonly Settings _settings;

        public BaseMessageReceiver(
            IMessagingHubSender sender,
            IContactService contactService,
            IResourceExtension resource,
            IUserContextService context,
            IEventTrackExtension track,
            Settings settings
            )
        {
            _sender = sender;
            _contactService = contactService;
            _resource = resource;
            _settings = settings;
            _context = context;
            _track = track;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var contact = await GetContact(userNode, cancellationToken);
            var context = await _context.GetContextAsync(userNode, cancellationToken);
            if (context == null)
            {
                context = new Models.UserContext();
                await _context.SetContextAsync(context, userNode, cancellationToken);
            }
            
            if (_contactService.IsContactFirstTime(contact))
            {
                await _track.AddAsync("Métricas de Usuários", "Novos usuários", cancellationToken: cancellationToken, identity: userNode);
                var welcomeMessageResource = await _resource.GetAsync<Document>(_settings.Resources.Welcome, cancellationToken);
                await _sender.SendMessageAsync(welcomeMessageResource, userNode, cancellationToken);
            }

            await ProcessAsync(envelope, cancellationToken);

            
        }

        public abstract Task ProcessAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken));

        private async Task<Contact> GetContact(Identity userNode, CancellationToken cancellationToken)
        {
            return await _contactService.GetContactAsync(userNode, cancellationToken);
        }
    }
}
