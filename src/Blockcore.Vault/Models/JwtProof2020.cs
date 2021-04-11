using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.Vault.Models
{
    public class JwtProof2020
    {
        public string Type => "JwtProof2020";

        public string Jwt { get; set; }
    }
}
