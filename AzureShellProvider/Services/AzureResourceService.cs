using AzureShellProvider.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureShellProvider.Services
{
    public class AzureResourceService
    {
        private readonly HttpClient _client;
        private IEnumerable<Subscription> _subscriptions;
        private IEnumerable<ResourceGroup> _resourceGroups;
        private IEnumerable<Resource> _resources;
        public Subscription CurrentSubscription { get; set; }
        public ResourceGroup CurrentResourceGroup { get; set; }
        public AzureResourceService(string accessToken)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://management.azure.com/")
            };
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions()
        {
            var response = await _client.GetStringAsync("/subscriptions?api-version=2014-04-01-preview").ConfigureAwait(false);
            _subscriptions = JToken.Parse(response).SelectToken("value").ToObject<IEnumerable<Subscription>>();
            return _subscriptions;
        }

        internal void SetCurrentSubscription(string name)
        {
            _subscriptions = _subscriptions ?? GetSubscriptions().Result;
            CurrentSubscription = _subscriptions.FirstOrDefault(s => s.DisplayName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        internal void SetCurrentResourceGroup(string name)
        {
            _resourceGroups = _resourceGroups ?? GetResourceGroups(CurrentSubscription.DisplayName).Result;
            CurrentResourceGroup = _resourceGroups.FirstOrDefault(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<IEnumerable<ResourceGroup>> GetResourceGroups(string subscriptionName)
        {
            var subscription = await GetSubscriptionId(subscriptionName);
            var response = await _client.GetStringAsync($"/subscriptions/{subscription}/resourceGroups?api-version=2016-07-01").ConfigureAwait(false);
            _resourceGroups = JToken.Parse(response).SelectToken("value").ToObject<IEnumerable<ResourceGroup>>();
            return _resourceGroups;
        }

        public async Task<IEnumerable<Resource>> GetResources(string subscriptionName, string resourceGroup)
        {
            var subscription = await GetSubscriptionId(subscriptionName).ConfigureAwait(false);
            var response = await _client.GetStringAsync($"/resources?$filter=subscriptionId%20EQ%20'{subscription}'%20AND%20substringof('{resourceGroup}',%20resourceGroup)&api-version=2016-09-01").ConfigureAwait(false);
            _resources = JToken.Parse(response).SelectToken("value").ToObject<IEnumerable<Resource>>();
            return _resources;
        }

        public Resource GetResourceBy(string name, string type)
        {
            return _resources.FirstOrDefault(r => r.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && r.Type.StartsWith(type, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Resource> GetResourcesBy(string type)
        {
            return _resources.Where(r => r.Type.StartsWith(type, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<string> GetSubscriptionId(string subscriptionName)
        {
            if(_subscriptions == null)
            {
                _subscriptions = await GetSubscriptions().ConfigureAwait(false);
            }
            return _subscriptions.FirstOrDefault(s => s.DisplayName == subscriptionName).SubscriptionId.ToString();
        }

        private async Task<KeyVault> GetKeyVaultAsync(Resource resource)
        {
            var response = await _client.GetStringAsync(resource.Id + "?api-version=2015-06-01").ConfigureAwait(false);
            return JToken.Parse(response).ToObject<IEnumerable<KeyVault>>().FirstOrDefault();
        }
        public async Task<IEnumerable<KeyVaultSecret>> GetSecretsAsync(Resource resource)
        {
            //var keyvault = await GetKeyVaultAsync(resource).ConfigureAwait(false);
            var response = await _client.GetStringAsync($"{resource.Id}/secrets?api-version=2015-06-01").ConfigureAwait(false);
            return JToken.Parse(response).SelectToken("value").ToObject<IEnumerable<KeyVaultSecret>>();
        }
    }
}
