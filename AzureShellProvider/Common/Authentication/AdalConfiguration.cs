using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureShellProvider.Common.Authentication
{
    public class AdalConfiguration
    {
        public const string PowerShellClientId = "1950a258-227b-4e31-a9cf-717495945fc2";
        public static readonly Uri PowerShellRedirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob");
        public const string EnableEbdMagicCookie = "site_id=501358&display=popup";
        public string AdEndpoint { get; set; }
        public bool ValidateAuthority { get; set; }
        public string AdDomain { get; set; }
        public string ClientId { get; set; }
        public Uri ClientRedirectUri { get; set; }
        public string ResourceClientUri { get; set; }
        //public TokenCache TokenCache { get; set; }
        public AdalConfiguration()
        {
            ClientId = PowerShellClientId;
            ClientRedirectUri = PowerShellRedirectUri;
            ValidateAuthority = true;
            AdEndpoint = string.Empty;
            ResourceClientUri = "https://management.core.windows.net/";
        }
    }
}
