using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Blockcore.Vault.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using NBitcoin.Crypto;
using NBitcoin;
using NBitcoin.DataEncoders;
using System.Reflection;
using NBitcoin.Secp256k1;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Microsoft.IdentityModel.Tokens;
using Blockcore.Vault.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Blockcore.Vault.Tests.Controllers
{
    // https://github.com/chaincoin/chaincoin/blob/0.16/src/chainparams.cpp
    public class Chaincoin : NetworkSetBase
    {
        public static Chaincoin Instance { get; } = new Chaincoin();

        public override string CryptoCode => "CHC";

        private Chaincoin()
        {
        }

        public class ChaincoinConsensusFactory : ConsensusFactory
        {
            private ChaincoinConsensusFactory()
            {
            }

            public static ChaincoinConsensusFactory Instance { get; } = new ChaincoinConsensusFactory();

            public override BlockHeader CreateBlockHeader()
            {
                return new ChaincoinBlockHeader();
            }
            public override Block CreateBlock()
            {
                return new ChaincoinBlock(new ChaincoinBlockHeader());
            }
        }


#pragma warning disable CS0618 // Type or member is obsolete
        public class ChaincoinBlockHeader : BlockHeader
        {
            // blob
            private static byte[] CalculateHash(byte[] data, int offset, int count)
            {
                return null;
                // return new HashX11.C11().ComputeBytes(data.Skip(offset).Take(count).ToArray());
            }

            protected override HashStreamBase CreateHashStream()
            {
                return BufferedHashStream.CreateFrom(CalculateHash);
            }
        }

        public class ChaincoinBlock : Block
        {
            public ChaincoinBlock(ChaincoinBlockHeader h) : base(h)
            {
            }
            public override ConsensusFactory GetConsensusFactory()
            {
                return Instance.Mainnet.Consensus.ConsensusFactory;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        protected override void PostInit()
        {
            RegisterDefaultCookiePath("Chaincoin", new FolderName() { TestnetFolder = "testnet4" });
        }
        public class ChaincoinMainnetAddressStringParser : NetworkStringParser
        {
            public override bool TryParse(string str, Network network, Type targetType, out IBitcoinString result)
            {
                if (str.StartsWith("xprv", StringComparison.OrdinalIgnoreCase) && targetType.GetTypeInfo().IsAssignableFrom(typeof(BitcoinExtKey).GetTypeInfo()))
                {
                    try
                    {
                        result = new BitcoinExtKey(str, network);
                        return true;
                    }
                    catch
                    {
                    }
                }
                if (str.StartsWith("xpub", StringComparison.OrdinalIgnoreCase) && targetType.GetTypeInfo().IsAssignableFrom(typeof(BitcoinExtPubKey).GetTypeInfo()))
                {
                    try
                    {
                        result = new BitcoinExtPubKey(str, network);
                        return true;
                    }
                    catch
                    {
                    }
                }
                return base.TryParse(str, network, targetType, out result);
            }
        }

        protected override NetworkBuilder CreateMainnet()
        {
            var builder = new NetworkBuilder();
            builder.SetConsensus(new Consensus()
            {
                SubsidyHalvingInterval = 700800,
                MajorityEnforceBlockUpgrade = 750,
                MajorityRejectBlockOutdated = 950,
                MajorityWindow = 1000,
                BIP34Hash = new uint256("0x00000012f1c40ff12a9e6b0e9076fe4fa7ad27012e256a5ad7bcb80dc02c0409"),
                PowLimit = new Target(new uint256("0x00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                MinimumChainWork = new uint256("0x00000000000000000000000000000000000000000000000004b643d48e088b67"),
                PowTargetTimespan = TimeSpan.FromSeconds(90),
                PowTargetSpacing = TimeSpan.FromSeconds(90),
                PowAllowMinDifficultyBlocks = false,
                CoinbaseMaturity = 100,
                PowNoRetargeting = false,
                RuleChangeActivationThreshold = 10752,
                MinerConfirmationWindow = 13440,
                ConsensusFactory = ChaincoinConsensusFactory.Instance,
                SupportSegwit = true
            })
            .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 55 })
            .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 117 })
            .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 55 + 128 })
            .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
            .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
            .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("chc"))
            .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("chc"))
            .SetNetworkStringParser(new ChaincoinMainnetAddressStringParser())
            .SetMagic(0x037AD2A3)
            .SetPort(11994)
            .SetRPCPort(11995)
            .SetMaxP2PVersion(70015)
            .SetName("chaincoin-main")
            .AddAlias("chaincoin-mainnet")
            .AddDNSSeeds(new[]
            {
                new DNSSeedData("chc1.hashunlimited.com", "chc1.hashunlimited.com"),
                new DNSSeedData("chc2.hashunlimited.com", "chc2.hashunlimited.com"),
                new DNSSeedData("seed1.chaincoin.org", "seed1.chaincoin.org"),
                new DNSSeedData("seed2.chaincoin.org", "seed2.chaincoin.org"),
                new DNSSeedData("seed3.chaincoin.org", "seed3.chaincoin.org"),
                new DNSSeedData("seed4.chaincoin.org", "seed4.chaincoin.org")
            })
            // .AddSeeds(new NetworkAddress[0])
            .SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efa0ce9da52ffff0f1e43cc217d0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000");
            return builder;
        }

        protected override NetworkBuilder CreateTestnet()
        {
            var builder = new NetworkBuilder();
            var res = builder.SetConsensus(new Consensus()
            {
                SubsidyHalvingInterval = 56600,
                MajorityEnforceBlockUpgrade = 51,
                MajorityRejectBlockOutdated = 75,
                MajorityWindow = 100,
                BIP34Hash = new uint256("0x00000352de593a01e0efcbaec00345ec80d20c7bd2024ec7c2beec048af0e6d9"),
                PowLimit = new Target(new uint256("0x00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                MinimumChainWork = new uint256("0x000000000000000000000000000000000000000000000000000000060e06d35d"),
                PowTargetTimespan = TimeSpan.FromSeconds(90),
                PowTargetSpacing = TimeSpan.FromSeconds(90),
                PowAllowMinDifficultyBlocks = true,
                CoinbaseMaturity = 30,
                PowNoRetargeting = false,
                RuleChangeActivationThreshold = 1512,
                MinerConfirmationWindow = 2016,
                ConsensusFactory = ChaincoinConsensusFactory.Instance,
                SupportSegwit = true
            })
            .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 80 })
            .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 44 })
            .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 88 + 128 })
            .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
            .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
            .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tchc"))
            .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tchc"))
            .SetMagic(0x0211C2FB)
            .SetPort(21994)
            .SetRPCPort(21995)
            .SetMaxP2PVersion(70015)
            .SetName("chaincoin-test")
            .AddAlias("chaincoin-testnet")
            .AddDNSSeeds(new[]
            {
                new DNSSeedData("testseed.hashunlimited.com",  "testseed.hashunlimited.com")
            })
            // .AddSeeds(new NetworkAddress[0])
            .SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efadae5494dffff7f20000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000");
            return builder;
        }

        protected override NetworkBuilder CreateRegtest()
        {
            var builder = new NetworkBuilder();
            builder.SetConsensus(new Consensus()
            {
                SubsidyHalvingInterval = 150,
                MajorityEnforceBlockUpgrade = 750,
                MajorityRejectBlockOutdated = 950,
                MajorityWindow = 1000,
                BIP34Hash = new uint256(),
                PowLimit = new Target(new uint256("0x7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
                MinimumChainWork = new uint256("0x0000000000000000000000000000000000000000000000000000000000000000"),
                PowTargetTimespan = TimeSpan.FromSeconds(90),
                PowTargetSpacing = TimeSpan.FromSeconds(90),
                PowAllowMinDifficultyBlocks = true,
                CoinbaseMaturity = 100,
                PowNoRetargeting = true,
                RuleChangeActivationThreshold = 108,
                MinerConfirmationWindow = 144,
                ConsensusFactory = ChaincoinConsensusFactory.Instance,
                SupportSegwit = true
            })
            .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 111 })
            .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 196 })
            .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 239 })
            .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
            .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
            .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("chcrt"))
            .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("chcrt"))
            .SetMagic(0x56C31FFC)
            .SetPort(18444)
            .SetRPCPort(18445)
            .SetMaxP2PVersion(70015)
            .SetName("chaincoin-reg")
            .AddAlias("chaincoin-regtest")
            .AddDNSSeeds(new DNSSeedData[0])
            // .AddSeeds(new NetworkAddress[0])
            .SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000887c5c20f3075215e164877a6de732695a13c0f8ec0fcf6296fa942487f96efadae5494dffff7f20000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d01044531382d30312d3134202d20416e74692d667261636b696e672063616d706169676e65727320636861696e207468656d73656c76657320746f20706574726f6c2070756d7073ffffffff0100105e5f00000000434104becedf6ebadd4596964d890f677f8d2e74fdcc313c6416434384a66d6d8758d1c92de272dc6713e4a81d98841dfdfdc95e204ba915447d2fe9313435c78af3e8ac00000000");
            return builder;
        }
    }

    public class VaultControllerTests : IClassFixture<AppTestFixture>
    {
        readonly AppTestFixture fixture;
        // readonly HttpClient client;
        private static HttpClient client;

        private static readonly string BASEURL = "/api/vault/";

        public VaultControllerTests(AppTestFixture fixture)
        {
            this.fixture = fixture;

            // Reuse the client cross tests.
            client ??= fixture.CreateClient();

            client.DefaultRequestHeaders.Add("Vault-Api-Key", "f6e12d45-deff-4e42-8615-1724a2b53f83");
        }

        [Fact]
        public void ValidateCustomCryptoProvider()
        {
            X9ECParameters secp256k1 = ECNamedCurveTable.GetByName("secp256k1");
            ECDomainParameters domainParams = new ECDomainParameters(secp256k1.Curve, secp256k1.G, secp256k1.N, secp256k1.H, secp256k1.GetSeed());

            const string d = "paZVIr8UkGROMSifXnepc3w_rHYrJgYnaXndJWkVE-g";
            const string x = "JK-taIACzlEAJt2o6paNF6THjuignkNp_2EqrYhwFE4";
            const string y = "eJQYEKQzLZ-Rj3VTX5-R70ir-CR9lh45DgVRrXkbrs0";

            var point = secp256k1.Curve.CreatePoint(
                new BigInteger(1, Base64UrlEncoder.DecodeBytes(x)),
                new BigInteger(1, Base64UrlEncoder.DecodeBytes(y)));

            var handler = new JsonWebTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                //ValidIssuer = Jwt.Issuer,
                //ValidAudience = Jwt.Audience,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,

                // IssuerSigningKeys = didDocument.GetSecurityKeys(),
                IssuerSigningKey = new BouncyCastleEcdsaSecurityKey(
                           new ECPublicKeyParameters(point, domainParams))
                    { KeyId = "1" },

                ValidateIssuerSigningKey = true,
            };

            var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NksifQ.eyJpYXQiOjE2MTc4ODA1NTgsImV4cCI6MjE0NzQ4MzY0NywidmMiOnsiQGNvbnRleHQiOlsiaHR0cHM6Ly93d3cudzMub3JnLzIwMTgvY3JlZGVudGlhbHMvdjEiLCJodHRwczovL2lkZW50aXR5LmZvdW5kYXRpb24vLndlbGwta25vd24vZGlkLWNvbmZpZ3VyYXRpb24vdjEiXSwidHlwZSI6WyJWZXJpZmlhYmxlQ3JlZGVudGlhbCIsIkRvbWFpbkxpbmthZ2VDcmVkZW50aWFsIl0sImNyZWRlbnRpYWxTdWJqZWN0Ijp7ImlkIjoiZGlkOmlzOlBNVzFLczdoNGJycE44RmREVkx3aFBES0o3TGRBN21WZGQiLCJvcmlnaW4iOiJodHRwczovL3d3dy5ibG9ja2NvcmUubmV0In19LCJuYmYiOjE2MTc4ODA1NTgsInN1YiI6ImRpZDppczpQTVcxS3M3aDRicnBOOEZkRFZMd2hQREtKN0xkQTdtVmRkIiwiaXNzIjoiZGlkOmlzOlBNVzFLczdoNGJycE44RmREVkx3aFBES0o3TGRBN21WZGQifQ.jjjpW9ZlAEB0V8xQrO25tu5LbkNaMjxpYxrA_twaJArGcpnpt3TGvQVKfCd7tR-5TxoQUs51IzVs8HY3G9qViA";

            var result = handler.ValidateToken(
               token,
               validationParameters
               );

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateSignature()
        {
            var jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NksifQ.eyJpYXQiOjE2MTc4ODA1NTgsImV4cCI6MjE0NzQ4MzY0NywidmMiOnsiQGNvbnRleHQiOlsiaHR0cHM6Ly93d3cudzMub3JnLzIwMTgvY3JlZGVudGlhbHMvdjEiLCJodHRwczovL2lkZW50aXR5LmZvdW5kYXRpb24vLndlbGwta25vd24vZGlkLWNvbmZpZ3VyYXRpb24vdjEiXSwidHlwZSI6WyJWZXJpZmlhYmxlQ3JlZGVudGlhbCIsIkRvbWFpbkxpbmthZ2VDcmVkZW50aWFsIl0sImNyZWRlbnRpYWxTdWJqZWN0Ijp7ImlkIjoiZGlkOmlzOlBNVzFLczdoNGJycE44RmREVkx3aFBES0o3TGRBN21WZGQiLCJvcmlnaW4iOiJodHRwczovL3d3dy5ibG9ja2NvcmUubmV0In19LCJuYmYiOjE2MTc4ODA1NTgsInN1YiI6ImRpZDppczpQTVcxS3M3aDRicnBOOEZkRFZMd2hQREtKN0xkQTdtVmRkIiwiaXNzIjoiZGlkOmlzOlBNVzFLczdoNGJycE44RmREVkx3aFBES0o3TGRBN21WZGQifQ.jjjpW9ZlAEB0V8xQrO25tu5LbkNaMjxpYxrA_twaJArGcpnpt3TGvQVKfCd7tR-5TxoQUs51IzVs8HY3G9qViA";

            var jwtArray = jwt.Split(".");

            var header = jwtArray[0];
            var payload = jwtArray[1];
            var signature = jwtArray[2];

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(jwt);

            // var signatureDecoded = System.Text.Encoding.ASCII.GetBytes(signature);
            var signatureDecoded = Base64UrlDecode(signature);
            // var signatureDecoded2 = Encoders.Base64.DecodeData(signature);

            var input = System.Text.Encoding.UTF8.GetBytes($"{header}.{payload}");

            var hash = Hashes.DoubleSHA256(input);

            var bitcoinAddress = (BitcoinPubKeyAddress)BitcoinPubKeyAddress.Create("PMW1Ks7h4brpN8FdDVLwhPDKJ7LdA7mVdd", Chaincoin.Instance.Mainnet);

            var privateKeyBytes = Encoders.Hex.DecodeData("a5a65522bf1490644e31289f5e77a9737c3fac762b2606276979dd25691513e8");

            var privateKey = new Key(privateKeyBytes);

            var signatureFromHash = privateKey.Sign(hash);

            var publicKey = new PubKey(Encoders.Base58.DecodeData("wAAADkMFQkqxaUPB8jGq4ZoJVsaK9Y5M8riM76zugM6d"));
            // var verified = publicKey.Verify(hash, signatureDecoded);
            
            SecpECDSASignature outSignature;
            SecpECDSASignature.TryCreateFromCompact(signatureDecoded, out outSignature);

            ECDSASignature outSignature5;
            var signature4 = ECDSASignature.FromDER(signatureDecoded);
            var signature5 = ECDSASignature.TryParseFromCompact(signatureDecoded, out outSignature5);

            // var signature2 = new ECDSASignature(signatureDecoded);
            // var signature3 = new ECDSASignature(signatureDecoded2);

            // Verify the signatures here.

            var verified = publicKey.Verify(hash, signatureFromHash);

            Assert.True(verified);
        }

        private string Base64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Regular base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        private byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new System.Exception(
             "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }


        [Fact]
        public async void AddDataVault()
        {
            // var didDocument = System.IO.File.ReadAllText(System.IO.Path.Combine("Data/did-configuration.json"));
            // var didContent = new StringContent(didDocument, Encoding.UTF8, "application/json");

            var didConfiguration = File.ReadAllText(Path.Combine("Data", "did-configuration.json"));
            // var didConfigurationDocument = JsonSerializer.Deserialize<DIDConfiguration>(didConfiguration);
            var didConfigurationDocument = JsonConvert.DeserializeObject<DIDConfiguration>(didConfiguration);

            var response = await client.PostAsJsonAsync(BASEURL, didConfigurationDocument);
            // response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //// The endpoint 
            //var content = new StringContent(didConfiguration, Encoding.UTF8, "application/json");

            //var response = await client.PostAsync(BASEURL, content);

            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}
