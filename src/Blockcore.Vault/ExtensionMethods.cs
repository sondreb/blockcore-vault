using Blockcore.Vault.Authorization;
using Blockcore.Vault.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Vault
{
    public static class ExtensionMethods
    {
        public static string EncodeBase64(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(this string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

        public static IEnumerable<SecurityKey> GetSecurityKeys(this DIDDocument value)
        {
            List<SecurityKey> keys = new List<SecurityKey>();

            foreach (var verificationMethod in value.VerificationMethod)
            {
                // We only supports EcdsaSecp256k1VerificationKey2019, skip all other keys.
                if (verificationMethod.Type != "EcdsaSecp256k1VerificationKey2019")
                {
                    continue;
                }

                var key = new ES256KSecurityKey(verificationMethod.PublicKeyBase58);
                keys.Add(key);
            }

            return keys.ToArray();
        }
    }
}
