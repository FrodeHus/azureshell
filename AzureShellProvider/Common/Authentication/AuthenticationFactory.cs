using Microsoft.Azure.Management.Resource.Fluent;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureShellProvider.Common.Authentication
{
    public static class AuthenticationFactory
    {
        public static Task<AuthenticationResult> AuthenticateAsync()
        {
            var config = new AdalConfiguration();
            return Task.Factory.StartNew(() => SafeAquireToken(config));
        }

        private static AuthenticationResult SafeAquireToken(AdalConfiguration config)
        {
            try
            {
                return DoAcquireToken(config);
            }
            catch (AdalException adalEx)
            {

            }
            catch (Exception threadEx)
            {

            }
            return null;
        }

        private static AuthenticationResult DoAcquireToken(AdalConfiguration config)
        {
            AuthenticationResult result;
            var context = new AuthenticationContext("https://login.microsoftonline.com/statoilsrm.onmicrosoft.com", false);
            result = context.AcquireToken(
                config.ResourceClientUri,
                config.ClientId,
                config.ClientRedirectUri,
                PromptBehavior.Always,
                UserIdentifier.AnyUser,
                AdalConfiguration.EnableEbdMagicCookie);

            return result;
        }

 
    }
}
