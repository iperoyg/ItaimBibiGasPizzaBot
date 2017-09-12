using Lime.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeGasPriceBot.Extensions
{
    public static class LimeContactExtension
    {
        public static bool IsContactFirstTime(this Contact contact)
        {
            return contact.Extras["firstTime"] == true.ToString();
        }
    }
}
