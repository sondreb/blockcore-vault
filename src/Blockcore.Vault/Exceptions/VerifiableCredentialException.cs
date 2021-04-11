using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Make sure these are available in the root namespace even though they are located inside the "Exceptions" folder.
namespace Blockcore.Vault
{
    public class VerifiableCredentialException : Exception
    {
        public VerifiableCredentialException(string message, Exception innerException = null) : base(message, innerException)
        {

        }
    }
}
