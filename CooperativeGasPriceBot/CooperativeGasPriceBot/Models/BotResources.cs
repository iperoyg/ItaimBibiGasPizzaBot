using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeGasPriceBot.Models
{
    [DataContract]
    public class BotResources
    {
        [DataMember(Name = "welcome")]
        public string Welcome { get; set; }
        [DataMember(Name = "menu")]
        public string Menu { get; set; }
        [DataMember(Name = "endMenu")]
        public string EndMenu { get; internal set; }
        [DataMember(Name = "notFoundStations")]
        public string NotFoundStations { get; internal set; }
        [DataMember(Name = "stopSetPrice")]
        public string StopSetPrice { get; internal set; }
        [DataMember(Name = "priceUpdated")]
        public string PriceUpdated { get; internal set; }
        [DataMember(Name = "notAPrice")]
        public string NotAPrice { get; internal set; }
        [DataMember(Name = "reportPriceLocation")]
        public string ReportPriceLocationRequest { get; internal set; }
        [DataMember(Name = "searchPriceLocation")]
        public string SearchPriceLocationRequest { get; internal set; }
        [DataMember(Name = "noneLovedStations")]
        public string NoneLovedStations { get; internal set; }
    }
}
