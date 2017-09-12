﻿using System;
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

namespace CooperativeGasPriceBot.Receivers
{
    public class BaseMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IContactService _contactService;
        private readonly IResourceExtension _resource;

        public BaseMessageReceiver(
            IMessagingHubSender sender,
            IContactService contactService,
            IResourceExtension resource
            )
        {
            _sender = sender;
            _contactService = contactService;
            _resource = resource;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userNode = envelope.From.ToIdentity();
            var contact = await GetContact(userNode, cancellationToken);

            if (_contactService.IsContactFirstTime(contact))
            {
                var welcomeMessageResource = await _resource.GetAsync<Document>("$welcome_message", cancellationToken); //$welcome_message
                await _sender.SendMessageAsync(welcomeMessageResource, userNode, cancellationToken);
            }

            var mainOptions = new Select
            {
                Text = "Escolha uma das opções abaixo:",
                Scope = SelectScope.Immediate, // QuickReply
                Options = new SelectOption[2]
                {
                    new SelectOption {  Order = 1, Text = "Pesquisar preços", Value = PlainText.Parse("/searchPrice")},
                    new SelectOption {  Order = 2, Text = "Informar preço", Value = PlainText.Parse("/reportPrice")},
                }
            };

            await _sender.SendMessageAsync(mainOptions, userNode, cancellationToken);

        }

        private async Task<Contact> GetContact(Identity userNode, CancellationToken cancellationToken)
        {
            return await _contactService.GetContactAsync(userNode, cancellationToken);
        }
    }
}
