using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeGasPriceBot.Models
{
    [DataContract]
    public class UserContext : Document
    {
        public static MediaType MEDIA_TYPE = MediaType.Parse("application/coopgaspricebot.usercontext+json");

        [DataMember(Name = "currentJourney")]
        public Journey CurrentJourney { get; set; }
        [DataMember(Name = "currentGasStation")]
        public int CurrentGasStationId { get; set; }
        public UserContext() : base(MEDIA_TYPE)
        {

        }

    }
}
