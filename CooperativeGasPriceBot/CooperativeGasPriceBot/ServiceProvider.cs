﻿using CooperativeGasPriceBot.Services;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.Host;

namespace CooperativeGasPriceBot
{
    public class ServiceProvider : Container, IServiceContainer
    {

        public ServiceProvider()
        {
            DefaultRegistration();
        }

        private void DefaultRegistration()
        {
            RegisterSingleton<IContactService, ContactService>();
            RegisterSingleton<IUserContextService, UserContextService>();
            RegisterSingleton<IGasStationService, GasStationService>();
        }

        public void RegisterService(Type serviceType, object instance)
        {
            RegisterSingleton(serviceType, instance);
        }

        public void RegisterService(Type serviceType, Func<object> instanceFactory)
        {
            RegisterSingleton(serviceType, instanceFactory);
        }
    }
}
