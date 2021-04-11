using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class VerifiableCredential
    {
        public VerifiableCredential()
        {
            Type = new List<string>();
            Type.Add("VerifiableCredential");

            Context = new List<string>();
            Context.Add("https://www.w3.org/2018/credentials/v1");
        }

        public List<string> Context { get; set; }

        public string Issuer { get; set; }

        public DateTime IssuanceDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public List<string> Type { get; set; }

        // Add support for various types of proof in the future, currently we only support the non-standard "JwtProof2020", since signatures are verfied as JWTs.
        public JwtProof2020 Proof { get; set; }

    }
}
