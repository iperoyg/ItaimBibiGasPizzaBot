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
    public class GasStation : Document
    {
        public static MediaType MEDIA_TYPE = MediaType.Parse("application/coopgaspricebot.gasstation+json");

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public decimal ActualPrice { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string PhotoUri { get; set; }
        [DataMember]
        public string Address { get; set; }
        
        public GasStation() : base(MEDIA_TYPE)
        {

        }

        public string GetNameId()
        {
            return $"gasstation_{Id}";
        }
    }
}
