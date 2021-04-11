using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class DomainLinkageCredentialSubject
    { 
        public string Id { get; set; }

        public string Origin { get; set; }
    }

    public class DomainLinkageCredential : VerifiableCredential
    {
        public DomainLinkageCredential() : base()
        {
            Context.Add("https://identity.foundation/.well-known/did-configuration/v1");
            Type.Add("DomainLinkageCredential");
        }

        public DomainLinkageCredentialSubject CredentialSubject { get; set; }
    }
}
