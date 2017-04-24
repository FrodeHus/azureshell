using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureShellProvider.Common
{
    public static class AzureSession
    {

        private static string GetProfileFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Windows Azure Powershell");
        }
    }
}
