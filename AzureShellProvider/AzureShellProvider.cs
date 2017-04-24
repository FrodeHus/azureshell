using AzureShellProvider.Common.Authentication;
using AzureShellProvider.Services;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System;
using System.Linq;
using AzureShellProvider.Model;

namespace AzureShellProvider
{
    [CmdletProvider("AzureShellProvider", ProviderCapabilities.None)]
    public class AzureShellProvider : NavigationCmdletProvider
    {
        private static AzureResourceService _azure;
        protected override bool IsValidPath(string path)
        {
            return true;
        }

        protected override Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            var drive = new PSDriveInfo("Azure", this.ProviderInfo, "", "", null);
            return new Collection<PSDriveInfo> { drive };
        }

        protected override bool ItemExists(string path)
        {
            EnsureAuthenticated();
            return true;
        }

        protected override bool IsItemContainer(string path)
        {
            EnsureAuthenticated();
            var pathItems = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathItems.Length == 0 || pathItems.Length == 1)
            {
                _azure.SetCurrentSubscription(path);
            }else if(pathItems.Length > 1)
            {
                _azure.SetCurrentResourceGroup(pathItems[1]);
            }
            return true;
        }
        
        private void EnsureAuthenticated()
        {
            if(_azure == null)
            {
                var token = AuthenticationFactory.AuthenticateAsync().Result;
                _azure = new AzureResourceService(token.AccessToken);
            }
        }
        
        protected override void GetChildItems(string path, bool recurse)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                WriteSubscriptions();
                return;
            }
            var pathItems = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if(pathItems.Length == 1)
            {
                WriteResourceGroups(path);
            }
            else if(pathItems.Length == 2)
            {
                WriteResourceTypes(pathItems[0], pathItems[1]);
            }
            else if(pathItems.Length == 3)
            {
                WriteResources(pathItems[2]);
            }
            else
            {
                WriteBasedOnResource(pathItems[3], pathItems[2]);
            }
        }

        private void WriteResources(string resourceType)
        {
            var resources = _azure.GetResourcesBy(resourceType);
            foreach(var resource in resources)
            {
                WriteItemObject(resource, resource.Name, true);
            }
        }

        private void WriteBasedOnResource(string resourceName, string type)
        {
            var resource = _azure.GetResourceBy(resourceName, type);
            if (resource.Type.Contains("KeyVault"))
            {
                WriteKeyVaultSecrets(resource);
            }
        }

        private void WriteKeyVaultSecrets(Resource keyvault)
        {
            foreach(var secret in _azure.GetSecretsAsync(keyvault).Result)
            {
                WriteItemObject(secret, secret.Name, false);
            }
        }

        private void WriteResourceTypes(string subscription, string resourceGroup)
        {
            var resources = _azure.GetResources(subscription, resourceGroup).Result;
            foreach(string type in resources.Select(r => r.Type.Substring(0, r.Type.IndexOf('/'))).Distinct())
            {
                WriteItemObject(type, type, true);
            }
        }

        private void WriteResourceGroups(string path)
        {
            var resourceGroups = _azure.GetResourceGroups(path).Result;
            foreach(var group in resourceGroups)
            {
                WriteItemObject(group, group.Name, true);
            }
        }

        private void WriteSubscriptions()
        {
            var subscriptions = _azure.GetSubscriptions().Result;
            foreach (var sub in subscriptions)
            {
                WriteItemObject(sub, sub.SubscriptionId.ToString(), true);
            }
        }
    }
}
