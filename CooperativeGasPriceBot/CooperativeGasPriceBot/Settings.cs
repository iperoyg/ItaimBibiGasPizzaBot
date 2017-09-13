using CooperativeGasPriceBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeGasPriceBot
{
    [DataContract]
    public class Settings
    {
        [DataMember(Name = "resources")]
        public BotResources Resources { get; set; }
    }
}
