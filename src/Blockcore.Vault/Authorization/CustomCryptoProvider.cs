using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Blockcore.Vault.Authorization
{
    public class CustomCryptoProvider : ICryptoProvider
    {
        public bool IsSupportedAlgorithm(string algorithm, params object[] args) => algorithm == "ES256K";

        public object Create(string algorithm, params object[] args)
        {
            if (algorithm == "ES256K"
                && args[0] is BouncyCastleEcdsaSecurityKey key)
            {
                return new CustomSignatureProvider(key, algorithm);
            }

            throw new NotSupportedException();
        }

        public void Release(object cryptoInstance)
        {
            if (cryptoInstance is IDisposable disposableObject)
                disposableObject.Dispose();
        }
    }

    public class CustomSignatureProvider : SignatureProvider
    {
        public CustomSignatureProvider(BouncyCastleEcdsaSecurityKey key, string algorithm)
            : base(key, algorithm) { }

        protected override void Dispose(bool disposing) { }

        public override byte[] Sign(byte[] input)
        {
            var ecDsaSigner = new ECDsaSigner();
            BouncyCastleEcdsaSecurityKey key = Key as BouncyCastleEcdsaSecurityKey;

            ecDsaSigner.Init(true, key.KeyParameters);

            byte[] hashedInput;
            using (var hasher = SHA256.Create())
            {
                hashedInput = hasher.ComputeHash(input);
            }

            var output = ecDsaSigner.GenerateSignature(hashedInput);

            var r = output[0].ToByteArrayUnsigned();
            var s = output[1].ToByteArrayUnsigned();

            var signature = new byte[r.Length + s.Length];
            r.CopyTo(signature, 0);
            s.CopyTo(signature, r.Length);

            return signature;
        }

        public override bool Verify(byte[] input, byte[] signature)
        {
            var ecDsaSigner = new ECDsaSigner();
            BouncyCastleEcdsaSecurityKey key = Key as BouncyCastleEcdsaSecurityKey;

            ecDsaSigner.Init(false, key.KeyParameters);

            byte[] hashedInput;
            using (var hasher = SHA256.Create())
            {
                hashedInput = hasher.ComputeHash(input);
            }

            var r = new BigInteger(1, signature.Take(32).ToArray());
            var s = new BigInteger(1, signature.Skip(32).ToArray());

            return ecDsaSigner.VerifySignature(hashedInput, r, s);
        }
    }

    public class BouncyCastleEcdsaSecurityKey : AsymmetricSecurityKey
    {
        public BouncyCastleEcdsaSecurityKey(ECKeyParameters keyParameters)
        {
            KeyParameters = keyParameters;
            CryptoProviderFactory.CustomCryptoProvider = new CustomCryptoProvider();
        }

        public ECKeyParameters KeyParameters { get; }

        public override int KeySize => throw new NotImplementedException();

        [Obsolete("HasPrivateKey method is deprecated, please use PrivateKeyStatus.")]
        public override bool HasPrivateKey => KeyParameters.IsPrivate;

        public override PrivateKeyStatus PrivateKeyStatus => KeyParameters.IsPrivate ? PrivateKeyStatus.Exists : PrivateKeyStatus.DoesNotExist;
    }
}
