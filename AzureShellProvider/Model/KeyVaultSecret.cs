﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureShellProvider.Model
{
    public class KeyVaultSecret
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public KeyVaultSecretProperties Properties { get; set; }
    }
}
