using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Sender;
using Takenet.MessagingHub.Client.Listener;
using System.Diagnostics;
using System;
using Lime.Protocol.Serialization;
using CooperativeGasPriceBot.Models;
using CooperativeGasPriceBot.Services;

namespace CooperativeGasPriceBot
{
	public class Startup : IStartable
	{
		private readonly IMessagingHubSender _sender;
		private readonly IDictionary<string, object> _settings;
        private readonly IGasStationService _gasStationService;

        public Startup(
            IMessagingHubSender sender, 
            IDictionary<string, object> settings,
            IGasStationService gasStationService
            )
		{
			_sender = sender;
			_settings = settings;
            _gasStationService = gasStationService;

        }

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

			TypeUtil.RegisterDocument<UserContext>();
            TypeUtil.RegisterDocument<GasStation>();

            await _gasStationService.InitializeAsync(cancellationToken);
            
		}
	}
}
