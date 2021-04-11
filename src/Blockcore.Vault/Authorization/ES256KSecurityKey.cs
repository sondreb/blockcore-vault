using Cryptography.ECDSA;
using Microsoft.IdentityModel.Tokens;
using NBitcoin;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.Secp256k1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Authorization
{
    public class ES256KSignatureProvider : SignatureProvider
    {
        public ES256KSignatureProvider(ES256KSecurityKey key, string algorithm) : base(key, algorithm)
        {

        }

        public override byte[] Sign(byte[] input)
        {
            return new byte[0];
            //var ecDsaSigner = new ECDsaSigner();
            //BouncyCastleEcdsaSecurityKey key = Key as BouncyCastleEcdsaSecurityKey;

            //ecDsaSigner.Init(true, key.KeyParameters);

            //byte[] hashedInput;
            //using (var hasher = SHA256.Create())
            //{
            //    hashedInput = hasher.ComputeHash(input);
            //}

            //var output = ecDsaSigner.GenerateSignature(hashedInput);

            //var r = output[0].ToByteArrayUnsigned();
            //var s = output[1].ToByteArrayUnsigned();

            //var signature = new byte[r.Length + s.Length];
            //r.CopyTo(signature, 0);
            //s.CopyTo(signature, r.Length);

            //return signature;
        }

        public override bool Verify(byte[] input, byte[] signature)
        {
            return true;

//            var key = (ES256KSecurityKey)Key;

//            X9ECParameters secp256k1 = ECNamedCurveTable.GetByName("secp256k1");
//            ECDomainParameters domainParams = new ECDomainParameters(secp256k1.Curve, secp256k1.G, secp256k1.N, secp256k1.H, secp256k1.GetSeed());

//            //var firstArray = signature.Take(signature.Length / 2).ToArray();
//            //var secondArray = signature.Skip(signature.Length / 2).ToArray();

//            //var point = secp256k1.Curve.CreatePoint(
//            //    new BigInteger(1, firstArray),
//            //    new BigInteger(1, secondArray));

//            var hash = Hashes.DoubleSHA256(input);

//            ECCurve curve = ConfigureCurveGlv(new SecP256K1Curve(), glv);


//            NBitcoin.Crypto.ECDSASignature()

//            var ecDsaSigner = new ECDsaSigner();

//            ecDsaSigner.Init(false, key.KeyParameters);

//            var r = new BigInteger(1, signature.Take(32).ToArray());
//            var s = new BigInteger(1, signature.Skip(32).ToArray());

//            ECDSASignature sig = new ECDSASignature(signature);
            

//            return ecDsaSigner.VerifySignature(hash, r, s);

//            //var point = secp256k1.Curve.CreatePoint(
//            //new BigInteger(1, Base64UrlEncoder.DecodeBytes(x)),
//            //new BigInteger(1, Base64UrlEncoder.DecodeBytes(y)));

//            var inputText = System.Text.Encoding.UTF8.GetString(input);
//            var signatureText = System.Text.Encoding.UTF8.GetString(signature);

////#if HAS_SPAN
////			var messageToSign = inputText;
////			var hash = Hashes.DoubleSHA256(messageToSign);
////			Span<byte> msg = stackalloc byte[32];
////			hash.ToBytes(msg);
////			return _ECKey.SigVerify(signature.ToSecpECDSASignature(), msg);
////#else
//            var messageToSign = inputText;

//            var hash = Hashes.DoubleSHA256(input);

//            // var recoveredPubKey = PubKey.RecoverCompact(hash, signature);

//            // ECPubKey.TryRecover()

//            //return ECKey.Verify(hash, signature);

////#endif

//            // byte[] signatureEncoded = Encoders.Base64.DecodeData(signatureText);
//            byte[] message = input;
//            //uint256 hash = Hashes.DoubleSHA256(message);

//            ECDSASignature signatureInstance;
//            var success = ECDSASignature.TryParseFromCompact(signature, out signatureInstance);

//            var verified = key.PublicKey.Verify(hash, signatureInstance);

//            //return RecoverCompact(hash, signatureEncoded);

//            if (!success)
//            {
//                return false;
//            }

//            var valid = key.PublicKey.VerifyMessage(input, signatureInstance);

//            return valid;

            //var message = Parseuint256(vector[3]);
            //var expectedSignature = SchnorrSignature.Parse(vector[4]);

            //var signature = privatekey.SignSchnorr(message);
            //Assert.Equal(expectedSignature.ToBytes(), signature.ToBytes());

            //Assert.True(publicKey.Verify(message, expectedSignature));


            //SecpECDSASignature signatureVerifier;
            //var success = NBitcoin.Secp256k1.ECPrivKey.TryCreate(.SecpECDSASignature.TryCreateFromCompact(signature, out signatureVerifier);

            //// NBitcoin.Secp256k1.ECPubKey.TryCreate(input, ctx, compressed, pubkey);

            //if (!success)
            //{
            //    throw new VerifiableCredentialException("Unable to restore signature validation from the signature value.");
            //}

            //Secp256K1Manager.SignCompact


            ////Sign message
            //var seckey = Hex.HexToBytes("80f3a375e00cc5147f30bee97bb5d54b31a12eee148a1ac31ac9edc4ecd13bc1f80cc8148e");
            //var data = Sha256Manager.GetHash(msg);
            //var sig = Secp256K1Manager.SignCompressedCompact(data, seckey);

            //var ecDsaSigner = new ECDsaSigner();
            //BouncyCastleEcdsaSecurityKey key = Key as BouncyCastleEcdsaSecurityKey;

            //ecDsaSigner.Init(false, key.KeyParameters);

            //byte[] hashedInput;
            //using (var hasher = SHA256.Create())
            //{
            //    hashedInput = hasher.ComputeHash(input);
            //}

            //var r = new BigInteger(1, signature.Take(32).ToArray());
            //var s = new BigInteger(1, signature.Skip(32).ToArray());

            //return ecDsaSigner.VerifySignature(hashedInput, r, s);
        }

        protected override void Dispose(bool disposing) { }
    }

    public class ES256KCryptoProvider : ICryptoProvider
    {
        public ES256KCryptoProvider()
        {

        }

        public object Create(string algorithm, params object[] args)
        {
            if (algorithm == "ES256K" && args[0] is ES256KSecurityKey key)
            {
                return new ES256KSignatureProvider(key, algorithm);
            }

            throw new NotSupportedException();
        }

        public bool IsSupportedAlgorithm(string algorithm, params object[] args) => algorithm == "ES256K";

        public void Release(object cryptoInstance)
        {
            if (cryptoInstance is IDisposable disposableObject)
                disposableObject.Dispose();
        }
    }

    public class ES256KSecurityKey : AsymmetricSecurityKey
    {
        public string PublicKeyBase58 { get; private set; }

        public PubKey PublicKey { get; private set; }

        public ES256KSecurityKey(string publicKeyBase58)
        {
            PublicKeyBase58 = publicKeyBase58;

            PublicKey = new PubKey(Encoders.Base58.DecodeData(PublicKeyBase58));

            CryptoProviderFactory.CustomCryptoProvider = new ES256KCryptoProvider();
        }

        public override bool HasPrivateKey => false;

        public override PrivateKeyStatus PrivateKeyStatus => PrivateKeyStatus.DoesNotExist;

        public override int KeySize => throw new NotImplementedException();
    }
}
