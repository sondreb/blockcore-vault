using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blockcore.Vault.Tests.Controllers
{
    public class VaultConcept
    { 
        public string Did { get; set; }
    }

    public class ConceptTests
    {
        public ConceptTests()
        {

        }

        [Fact]
        public void ConceptDemo()
        {
            VaultConcept vault1 = new VaultConcept();
            VaultConcept vault2 = new VaultConcept();

        }
    }
}
