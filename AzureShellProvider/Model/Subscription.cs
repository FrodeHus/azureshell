using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureShellProvider.Model
{
    public class Subscription
    {
        public string Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
    }
}
