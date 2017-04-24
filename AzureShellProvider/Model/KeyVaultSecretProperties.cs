using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureShellProvider.Model
{
    public class KeyVaultSecretProperties
    {
        public string ContentType { get; set; }
        public string SecretUri { get; set; }
        public string SecretUriWithVersion { get; set; }
    }
}
